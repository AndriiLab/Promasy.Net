<template>
  <div class="grid grid-cols-12 gap-8">
    <div class="col-span-12">
      <div class="card">
        <CpvSelector :select-mode="false" v-model="model"></CpvSelector>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import CpvSelector from "../components/CpvSelector.vue";
import {Cpv} from "@/services/api/cpv";
import {ref} from "vue";

const model = ref<Cpv>({} as Cpv);
</script>

