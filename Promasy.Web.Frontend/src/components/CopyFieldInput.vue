<template>
  <div class="field-group">
    <div class="field-input">
      <div class="text-bold">{{props.label}}</div>
      <div>{{props.text}}</div>
    </div>
    <div class="field-action">
      <Button v-if="props.enabled" icon="pi pi-copy" class="p-button-outlined" @click="onButtonClick" v-tooltip.bottom="t('copy')" />
      <i v-else class="pi pi-check-square mr-2" style="font-size: 2rem; color: #22C55E"></i>
    </div>
  </div>
  
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const emit = defineEmits([ "selected" ]);
const props = defineProps<{
  label: string,
  enabled: boolean,
  text: any
}>();

function onButtonClick() {
  emit("selected");
}
</script>

<style lang="scss" scoped>
.field-group {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.field-input {
  height: 100%;
}
.field-action {
  margin-left: 5px;
}
.text-bold {
  font-weight: bold;
}
</style>