<template>
  <Password v-model="value" :placeholder="placeholder" :toggleMask="true"
            :inputClass="inputClasses"
            :inputStyle="inputStyle" :feedback="feedback"
            :promptLabel="promptLabel ?? t('enterPassword')" :weakLabel="t('weakPassword')"
            :mediumLabel="t('mediumPassword')" :goodLabel="t('goodPassword')"
            :strongLabel="t('strongPassword')"></Password>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const emit = defineEmits([ "update:modelValue" ]);
const { t } = useI18n({ useScope: "local" });
const props = defineProps<{
  modelValue: string,
  feedback?: boolean,
  inputClasses?: string[],
  inputStyle?: string,
  placeholder?: string,
  promptLabel?: string,
  selectorClasses?: string[],
}>();

const value = computed({
  get() {
    return props.modelValue;
  },
  set(value) {
    emit("update:modelValue", value);
  },
});
</script>

<style scoped>
.pi-eye {
  transform: scale(1.6);
  margin-right: 1rem;
}

.pi-eye-slash {
  transform: scale(1.6);
  margin-right: 1rem;
}
</style>

<i18n locale="en">
{
  "enterPassword": "Enter a password",
  "weakPassword": "Weak",
  "mediumPassword": "Medium",
  "goodPassword": "Good",
  "strongPassword": "Strong"
}
</i18n>

<i18n locale="uk">
{
  "enterPassword": "Введіть пароль",
  "weakPassword": "Слабкий",
  "mediumPassword": "Середній",
  "goodPassword": "Хороший",
  "strongPassword": "Сильний"
}
</i18n>