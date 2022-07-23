<template>
  <div class="p-inputgroup">
    <span class="p-inputgroup-addon">₴</span>
    <InputText :id="props.inputId" :placeholder="inputPlaceholder" v-model.number="value" min="0" step="1"/>
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import Currency from "currency.js";

const emit = defineEmits([ "update:modelValue" ]);

const props = defineProps<{
  modelValue: number | undefined,
  inputId: string,
  inputPlaceholder: string | undefined
}>();

const value = computed({
  get() {
    return !!props.modelValue ? props.modelValue : 0;
  },
  set(value) {
    emit("update:modelValue", Currency(value).value);
  },
});
</script>