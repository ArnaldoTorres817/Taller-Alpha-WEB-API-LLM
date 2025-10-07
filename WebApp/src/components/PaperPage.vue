<template>
  <div class="paper-page" :style="customStyle">
    <slot />
  </div>
</template>

<script setup>
defineProps({
  customStyle: {
    type: Object,
    default: () => ({})
  }
});
</script>

<style scoped>
.paper-page {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%) /* plus your custom transform */;

  width: 500px;
  min-width: 0;
  max-width: 95vw;
  min-height: 750px;
  max-height: 90vh;
  margin: 0 auto;
  padding: 40px 32px;
  background:
    url('https://www.transparenttextures.com/patterns/paper-fibers.png'),
    linear-gradient(135deg, #f8f5e4 90%, #ece3c3 100%);
  filter: contrast(1.08) brightness(0.98);
  background-blend-mode: multiply, multiply;
  background-size: 400px 400px, cover;
  font-family: 'Georgia', 'Times New Roman', serif;
  color: #4b3e2e;
  box-sizing: border-box;
  box-shadow: 0 12px 32px 0 rgba(0, 0, 0, 0.18), 0 2px 12px 0 rgba(80, 60, 20, 0.10);
  overflow: auto;
  transition: box-shadow 0.2s;
}
</style>