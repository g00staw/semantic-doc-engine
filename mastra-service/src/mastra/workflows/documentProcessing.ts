import { createWorkflow, createStep } from '@mastra/core/workflows';
import { z } from 'zod';
import { ollama } from 'ollama-ai-provider-v2';
import { embedMany } from 'ai';
import { db } from '../../db.js';

// ─── Chunking ─────────────────────────────────────────────────────────────────
// 1 token ≈ 4 characters, so:
// 500 tokens  ≈ 2000 chars (chunk size)
//  50 tokens  ≈  200 chars (overlap)

const CHUNK_CHARS = 2000;
const OVERLAP_CHARS = 200;

function chunkText(text: string): string[] {
  const chunks: string[] = [];
  let start = 0;

  while (start < text.length) {
    const end = Math.min(start + CHUNK_CHARS, text.length);
    chunks.push(text.slice(start, end));
    if (end === text.length) break;
    start += CHUNK_CHARS - OVERLAP_CHARS;
  }

  return chunks;
}

// ─── Step 1: Split text into overlapping chunks ───────────────────────────────

const chunkTextStep = createStep({
  id: 'chunk-text',
  inputSchema: z.object({
    documentId: z.string().uuid(),
    text: z.string().min(1),
  }),
  outputSchema: z.object({
    documentId: z.string().uuid(),
    chunks: z.array(z.string()),
  }),
  execute: async ({ inputData }) => ({
    documentId: inputData.documentId,
    chunks: chunkText(inputData.text),
  }),
});

// ─── Step 2: Generate embeddings via Ollama ───────────────────────────────────

const generateEmbeddingsStep = createStep({
  id: 'generate-embeddings',
  inputSchema: z.object({
    documentId: z.string().uuid(),
    chunks: z.array(z.string()),
  }),
  outputSchema: z.object({
    documentId: z.string().uuid(),
    chunks: z.array(z.string()),
    embeddings: z.array(z.array(z.number())),
  }),
  execute: async ({ inputData }) => {
    const { embeddings } = await embedMany({
      model: ollama.textEmbeddingModel('nomic-embed-text'),
      values: inputData.chunks,
    });

    return {
      documentId: inputData.documentId,
      chunks: inputData.chunks,
      embeddings,
    };
  },
});

// ─── Step 3: Save chunks + embeddings to database ────────────────────────────

const saveChunksStep = createStep({
  id: 'save-chunks',
  inputSchema: z.object({
    documentId: z.string().uuid(),
    chunks: z.array(z.string()),
    embeddings: z.array(z.array(z.number())),
  }),
  outputSchema: z.object({
    documentId: z.string().uuid(),
    chunksCount: z.number(),
  }),
  execute: async ({ inputData }) => {
    const { documentId, chunks, embeddings } = inputData;

    for (let i = 0; i < chunks.length; i++) {
      await db.query(
        `INSERT INTO "DocumentChunks" ("Id", "DocumentId", "Content", "Embedding")
         VALUES ($1, $2, $3, $4::vector)`,
        [crypto.randomUUID(), documentId, chunks[i], JSON.stringify(embeddings[i])]
      );
    }

    await db.query(
      `UPDATE "Documents" SET "Status" = 'Completed' WHERE "Id" = $1`,
      [documentId]
    );

    return {
      documentId,
      chunksCount: chunks.length,
    };
  },
});

// ─── Workflow ─────────────────────────────────────────────────────────────────

export const documentProcessingWorkflow = createWorkflow({
  id: 'document-processing',
  inputSchema: z.object({
    documentId: z.string().uuid(),
    text: z.string().min(1),
  }),
  outputSchema: z.object({
    documentId: z.string().uuid(),
    chunksCount: z.number(),
  }),
})
  .then(chunkTextStep)
  .then(generateEmbeddingsStep)
  .then(saveChunksStep)
  .commit();
