<script setup>
import { ref } from 'vue';
import Book from './Book.vue';
import Chat from './Chat.vue'
defineProps({})

const prompt = ref('');

const promptReady = ref(false);

</script>

<template>
  <Chat v-model="prompt" @send="() => promptReady = true" />

  <template v-if="promptReady">
    <Book :prompt="prompt" />
  </template>
</template>

<style scoped>
h1 {
  font-weight: 500;
  font-size: 2.6rem;
  position: relative;
  top: -10px;
}

h3 {
  font-size: 1.2rem;
}
</style>