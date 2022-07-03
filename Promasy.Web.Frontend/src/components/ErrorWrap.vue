<template>
    <div :id="id">
        <slot></slot>
        <small v-for="err in errors" :key="err.$uid" class="p-error block">{{ err.$message }}</small>
        <small v-for="err in externalErrors" :key="getRandomId()" class="p-error block">{{ err }}</small>
    </div>
</template>

<script lang="ts" setup>
import { getRandomId } from "@/utils/randomization-utils";
import { ErrorObject } from "@vuelidate/core";
import { watch, computed } from "vue";

const inputErrorClass = "p-invalid";
const props = defineProps<{
    errors: ErrorObject[],
    externalErrors?: string[]
}>();
const id = getRandomId();
const hasErrors = computed(() => !!props.errors.length || !!props.externalErrors?.length );
watch(hasErrors, () => {
    const slotInputs = document.getElementById(id)?.querySelectorAll("input, select, textarea");
    slotInputs?.forEach((si) => {
        const hasErrorClass = si.classList.contains(inputErrorClass);
        if (!hasErrors && hasErrorClass) {
            si.classList.remove(inputErrorClass);
            return;
        }
        if (hasErrors && !hasErrorClass) {
            si.classList.add(inputErrorClass);
        }
    });
});
</script>