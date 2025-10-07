// Typewriter for a single chunk
export async function typewriterChunk(chunk, delay, appendFn) {
  for (let i = 0; i < chunk.length; i++) {
    appendFn(chunk[i]);
    await new Promise(r => setTimeout(r, delay));
  }
}

// Handles the queue of chunks
export async function processQueue(chunkQueue, delay, appendFn, typingFlag) {
  if (typingFlag.value) return;
  typingFlag.value = true;
  while (chunkQueue.length > 0) {
    const chunk = chunkQueue.shift();
    await typewriterChunk(chunk, delay.value, appendFn);
  }
  typingFlag.value = false;
}