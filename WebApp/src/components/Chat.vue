<template>
  <div class="flex-container-center text-align">
    <p class="title">Escribe un título para su cuento</p>
    <div class="chat-input-container">
      <input type="text" placeholder="Escribe aquí..." class="chat-input" :value="modelValue" @input="$emit('update:modelValue', $event.target.value)" />
      <button class="send-button" @click="$emit('send', modelValue)" :disabled="!modelValue || modelValue.trim() === ''">
        <svg xmlns="http://www.w3.org/2000/svg" height="24" width="24" viewBox="0 0 20 20" fill="currentColor">
          <path d="M2.5 17.5l15-7.5-15-7.5v6.25l10 1.25-10 1.25z"/>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  modelValue: String
})

// Emitting events for v-model and send action
const emit = defineEmits(['update:modelValue', 'send']);

</script>

<style scoped>
.title {
  font-size: 1.8rem;
  font-weight: 500;
}

.flex-container-center {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  gap: 1rem;
}

.text-align h1,
.text-align h3 {
  text-align: center;
}

@media (min-width: 1024px) {
  .text-align h1,
  .text-align h3 {
    text-align: left;
  }
}

.chat-input-container {
  width: 100%;
  height: 4rem;
  max-height: max(35svh,5rem);
  display: flex;
  align-items: center;
  margin-top: 1rem;
  background-color: #1a1a1a;
  border-radius: 20px;
  border-color: var(--main-bg-color);
  border-width: 4px;
  padding: 0.2rem 0.4rem 0.2rem 1rem;
  gap: 2em;
  box-shadow: 0 4px 24px 0 rgba(0, 0, 0, 0.35), 0 1.5px 8px 0 rgba(80, 0, 80, 0.18);
}

.chat-input {
  flex: 1;
  padding: 0.5rem 1rem 0.5rem 0;
  border: none;
  background: transparent;
  outline: none;
  font-size: 1.1rem;
  color: white;
}

.send-button {
  margin-left: 0.5rem;
  padding: 0.5rem 1.2rem;
  border: none;
  border-radius: 16px;
  background-color: transparent;
  color: white;
  font-weight: bold;
  cursor: pointer;
  font-size: 1rem;
  transition: background 0.2s;
}
</style>