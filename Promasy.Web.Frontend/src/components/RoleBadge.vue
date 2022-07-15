<template>
  <Tag :value="getRoleName(role)" :severity="getColor()"></Tag>
</template>

<script lang="ts" setup>
import { useRolesStore } from "@/store/roles";

const { getRoleName } = useRolesStore();
const props = defineProps<{
  role: number,
}>();

function getColor(){
  switch (props.role){
    case 1:
      return 'danger';
    case 2:
    case 3:
      return 'success';
    case 4:
    case 5:
    case 6:
    case 7:
      return 'warning';
    case 8:
    case 9:
      return 'success';
    default:
      return undefined;
  }
}
</script>
