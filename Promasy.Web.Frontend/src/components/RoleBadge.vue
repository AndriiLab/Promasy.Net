<template>
  <Tag class="role-tag" :value="getRoleName(role)" :style="{ 'background-color': backgroundColor, color: color }"></Tag>
</template>

<script lang="ts" setup>
import {defineProps, ref, watch} from "vue";
import {RoleEnum, getRoleName} from "@/constants/RoleEnum";
import {getColorPair} from "@/utils/color-utils";

const props = defineProps<{
  role: RoleEnum,
}>();


let pair = getColorPair(props.role + 200);
const backgroundColor = ref(pair.backgroundColor);
const color = ref(pair.foregroundColor);

watch(() => props.role, (val) => {
  pair = getColorPair(val + 200);
  backgroundColor.value = pair.backgroundColor;
  color.value = pair.foregroundColor
});
</script>

<style lang="scss">
.role-tag {
  -webkit-user-select: none;
  -ms-user-select: none;
  user-select: none;
}
</style>

