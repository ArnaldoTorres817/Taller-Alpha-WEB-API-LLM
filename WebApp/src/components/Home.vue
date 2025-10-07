<script setup>
import { ref } from 'vue';
import Book from './Book.vue';
import Chat from './Chat.vue'
defineProps({})

const prompt = ref('');

const promptReady = ref(false);
const bookKey = ref(0);

const handleSend = () => {
  promptReady.value = false; // Unmount Book
  bookKey.value++;           // Change key to force remount
  promptReady.value = true;  // Remount Book
};

</script>

<template>
  <Chat v-model="prompt" @cleartext="() => prompt = ''" @send="handleSend" />

  <template v-if="promptReady">
    <Book :prompt="prompt" :key="bookKey" />
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