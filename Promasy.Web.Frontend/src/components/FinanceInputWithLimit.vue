<template>
  <div class="p-inputgroup">
    <span class="p-inputgroup-addon">{{ t('available', { sum: limitLeft.format() }) }}</span>
    <span class="p-inputgroup-addon">₴</span>
    <InputText :id="props.inputId" :placeholder="inputPlaceholder" v-model.number="value" min="0" step="1"/>
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import currency from "@/utils/currency-utils";
import { useI18n } from "vue-i18n";

const emit = defineEmits([ "update:modelValue" ]);

const props = defineProps<{
  modelValue: number | undefined,
  limit: number,
  inputId: string,
  inputPlaceholder: string | undefined
}>();
const { t } = useI18n();
const limitLeft = computed(() => {
  return currency(props.limit).subtract(props.modelValue ?? 0);
});

const value = computed({
  get() {
    return !!props.modelValue ? props.modelValue : 0;
  },
  set(value) {
    emit("update:modelValue", limitLeft.value.value < 0 ? props.limit : currency(value).value);
  },
});
</script>

<i18n locale="en">
{
  "available": "{sum} avail."
}
</i18n>

<i18n locale="uk">
{
  "available": "Доступно {sum}"
}
</i18n>