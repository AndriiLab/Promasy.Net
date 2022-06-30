<template>
    <div :id="id">
        <slot></slot>
        <small v-for="err in errors" :key="err.$uid" class="p-error block">{{ err.$message }}</small>
    </div>
</template>

<script lang="ts" setup>
import { ErrorObject } from "@vuelidate/core";
import { watch } from "vue";

const inputErrorClass = "p-invalid";
const props = defineProps<{
    errors: ErrorObject[]
}>();
const id = Math.random().toString(36).slice(2);
watch(() => props.errors, (errs) => {
    const slotInputs = document.getElementById(id)?.querySelectorAll("input, select, checkbox, textarea");
    const hasError = !!errs.length;
    slotInputs?.forEach((si) => {
        const hasErrorClass = si.classList.contains(inputErrorClass);
        if (!hasError && hasErrorClass) {
            si.classList.remove(inputErrorClass);
            return;
        }
        if (hasError && !hasErrorClass) {
            si.classList.add(inputErrorClass);
        }
    });
});
</script>