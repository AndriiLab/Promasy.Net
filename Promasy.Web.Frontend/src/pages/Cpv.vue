<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">
        <CpvSelector :select-mode="false" v-model="model"></CpvSelector>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import CpvSelector from "../components/CpvSelector.vue";
import {Cpv} from "@/services/api/cpv";

const model = {} as Cpv;
</script>