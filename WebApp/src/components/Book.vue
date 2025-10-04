<template>
  <div class="book-stack">
    <template v-if="currentPage < streamedPages.length - 1">
      <PaperPage v-for="n in streamedPages.length - 1" :key="n" :customStyle="getPageStyle(n)" />
    </template>
    <PaperPage :customStyle="getPageStyle(0)">
      <div class="book-content">
        {{ streamedPages[currentPage] }}
      </div>
    </PaperPage>
  </div>
  <div class="navigator">
    <button class="nav-btn" @click="prevPage" :disabled="currentPage === 0">
      <svg aria-label="Previous" title="Previous" xmlns="http://www.w3.org/2000/svg" width="1.4em" height="1.4em"
        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
        stroke-linejoin="round">
        <polyline points="15 18 9 12 15 6" />
      </svg>
    </button>
    <span class="page-indicator">
      Page <b>{{ currentPage + 1 }}</b> / {{ streamedPages.length }}
    </span>
    <button class="nav-btn" @click="nextPage" :disabled="currentPage === streamedPages.length - 1">
      <svg aria-label="Next" title="Next" xmlns="http://www.w3.org/2000/svg" width="1.4em" height="1.4em"
        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
        stroke-linejoin="round">
        <polyline points="9 18 15 12 9 6" />
      </svg>
    </button>
  </div>
</template>

<script setup>
import { onMounted, ref, shallowRef } from 'vue'
import { processQueue } from '../utils/streamUtils.js'
import PaperPage from './PaperPage.vue'

const streamedPages = ref(['']); // Array of page texts
const currentPage = ref(0);
const typeDelay = ref(5); // ms per character

let typing = ref(false);
let chunkQueue = [];
const PAGE_CHAR_LIMIT = 1000; // Adjust as needed

const props = defineProps({
  prompt: {
    type: String,
    required: true
  }
});

const streamCache = shallowRef(new Map());

function appendCharToPages(char) {
  let pageText = streamedPages.value[streamedPages.value.length - 1];
  if (pageText.length >= PAGE_CHAR_LIMIT) {
    // Start a new page
    streamedPages.value[streamedPages.value.length - 1] += '...';

    streamedPages.value.push('');
    pageText = '';
  }

  streamedPages.value[streamedPages.value.length - 1] += char.replace(/\n/g, '. ');
}

async function streamText(url = 'http://localhost:5093/api/stream') {
  currentPage.value = 0;
  chunkQueue = [];
  typing.value = false;

  // Check cache
  if (streamCache.value.has(url)) {
    streamedPages.value = streamCache.value.get(url);
    return;
  }

  let fullText = '';

  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      userPrompt: preparePrompt(props.prompt)
    }),
  });

  const reader = response.body.getReader();
  const decoder = new TextDecoder();

  while (true) {
    const { value, done } = await reader.read();
    if (done) break;
    const chunk = decoder.decode(value, { stream: true });
    fullText += chunk;
    chunkQueue.push(chunk);
    processQueue(chunkQueue, typeDelay, appendCharToPages, typing);
  }

  // Wait for all chunks to finish typing before caching
  while (typing.value) {
    await new Promise(r => setTimeout(r, 10));
  }
  // Cache as a copy of the array
  streamCache.value.set(url, [...streamedPages.value]);
}

function preparePrompt(userPrompt) {
  return `Eres un escritor creativo y talentoso. Crea un cuento corto basado en el siguiente titulo: "${userPrompt}".
  El cuento debe ser apropiado para todas las edades, con un lenguaje claro y atractivo. 
  Limita la extensión del cuento a aproximadamente 200 palabras. 
  Evita incluir contenido inapropiado o sensible. /no_think`;
}

// Navigation
function prevPage() {
  if (currentPage.value > 0) currentPage.value--;
}
function nextPage() {
  if (currentPage.value < streamedPages.value.length - 1) currentPage.value++;
}

function getPageStyle(index) {
  // index 0 is the top (current) page
  if (index === 0) {
    return {
      zIndex: 100,
      boxShadow: '0 12px 32px 0 rgba(0,0,0,0.25)',
      transform: 'translate(-50%, -50%) translate(0, 0)',
      opacity: 1
    }
  }

  const totalPages = streamedPages.value.length;
  const remainingPages = totalPages - currentPage.value - 1;
  const maxPeekPages = 4;
  // Only peek as many as remaining, but never more than maxPeekPages
  const peekPages = Math.min(remainingPages, maxPeekPages);

  // If this page should not be shown as a peek, hide it
  if (index > peekPages) {
    return { display: 'none' };
  }

  // Spread peeking pages
  const peekIndex = index - 1; // 0-based for peeks
  const baseX = 4 + 4 * peekIndex;
  const baseY = 2 + 2 * peekIndex;
  const bright = 1 - 0.07 * peekIndex;

  return {
    zIndex: 99 - peekIndex,
    transform: `translate(-50%, -50%) translate(${baseX}px, ${baseY}px)`,
    filter: `brightness(${bright})`,
    opacity: 0.7 + 0.1 * (maxPeekPages - peekIndex)
  }
}

onMounted(() => {
  streamText();
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Kalam:wght@300;400;700&display=swap');

.book-stack {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  min-width: min-content;
  min-height: 650px;
  margin: 48px auto;
  height: 900px;
  max-width: 98vw;
}

.book-content {
  white-space: pre-wrap;
  font-size: 1.15rem;
  line-height: 1.7;
  letter-spacing: 0.01em;
  font-family: 'Kalam', cursive, 'Georgia', 'Times New Roman', serif;
}

.navigator {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1.5em;
  margin-top: 2em;
  font-family: 'Kalam', cursive, 'Georgia', serif;
  font-size: 1.15rem;
  background: rgba(243, 236, 215, 0.85);
  border-radius: 16px;
  box-shadow: 0 2px 8px 0 rgba(80, 60, 20, 0.10);
  padding: 0.5em 1.5em;
  width: fit-content;
  margin-left: auto;
  margin-right: auto;
}

.nav-btn {
  background: #e0d6c3;
  border: none;
  border-radius: 50%;
  width: 2.4em;
  height: 2.4em;
  font-size: 1.5em;
  color: #4b3e2e;
  cursor: pointer;
  transition: background 0.2s, box-shadow 0.2s;
  box-shadow: 0 1px 4px 0 rgba(80, 60, 20, 0.10);
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-btn:disabled {
  background: #f3ecd7;
  color: #b0a89a;
  cursor: not-allowed;
  opacity: 0.7;
}

.nav-btn:not(:disabled):hover {
  background: #f7e7c5;
  box-shadow: 0 2px 8px 0 rgba(80, 60, 20, 0.18);
}

.page-indicator {
  font-size: 1.1em;
  letter-spacing: 0.03em;
  color: #6d5c3d;
  background: #f8f5e4;
  border-radius: 8px;
  padding: 0.2em 0.8em;
  box-shadow: 0 1px 2px 0 rgba(80, 60, 20, 0.06);
}

@media (max-width: 600px) {
  .book-stack {
    width: 99vw;
    min-height: 60vw;
    height: auto;
  }

  .book-content {
    font-size: 1rem;
  }
}
</style>