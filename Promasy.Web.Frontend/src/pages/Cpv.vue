<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">
        <CpvSelector :select-mode="false" v-model="undefined"></CpvSelector>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import CpvSelector from "../components/CpvSelector.vue";
</script>