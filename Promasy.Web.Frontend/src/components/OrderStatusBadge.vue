<template>
  <Tag :value="getName(status)" :severity="getColor()"></Tag>
</template>

<script lang="ts" setup>
import { SelectItem } from "@/utils/fetch-utils";

const props = defineProps<{
  status: number,
  statuses: SelectItem<number>[]
}>();

function getColor(){
  switch (props.status){
    case 1:
    case 2:
      return 'info';
    case 3:
      return 'warning';
    case 4:
      return 'success';
    case 10:
    case 20:
      return 'danger';
    default:
      return undefined;
  }
}

function getName(status: number){
  return props.statuses.find(s => s.value === status)?.text;
}
</script>
