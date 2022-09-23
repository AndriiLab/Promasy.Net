<template>
  <div class="p-inputgroup">
    <slot name="prefix">
      <span class="p-inputgroup-addon">₴</span>
    </slot>
    <InputText :id="props.inputId" :placeholder="inputPlaceholder" v-model.number="value" min="0" step="1"/>
    <slot name="suffix"></slot>
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import currency from "@/utils/currency-utils";

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
    emit("update:modelValue", currency(value).value);
  },
});
</script>