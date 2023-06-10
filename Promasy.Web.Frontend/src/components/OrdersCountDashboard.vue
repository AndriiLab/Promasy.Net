<template>
  <div class="card mb-0">
    <div class="flex justify-content-between mb-3">
      <div>
        <span class="block text-500 font-medium mb-3">{{
            t(sessionStore.year === new Date().getFullYear() ? 'orders' : 'ordersInYear', {year: sessionStore.year})
          }}</span>
        <Skeleton v-if="isLoading" width="5rem" class="mb-2"></Skeleton>
        <div v-else class="text-900 font-medium text-xl">
          {{ total }}
        </div>
      </div>
      <div class="flex align-items-center justify-content-center border-round"
           :style="{ width:'2.5rem', height:'2.5rem', backgroundColor: pair.backgroundColor }">
        <i class="pi pi-shopping-cart text-xl" :style="{ color: pair.foregroundColor }"></i>
      </div>
    </div>
    <Skeleton v-if="isLoading" width="2rem" class="mb-2"></Skeleton>
    <div v-else>
      <span class="text-green-500 font-medium mr-1">
      {{ countByPeriod }}
      </span>
      <span class="text-500">{{ t('newForLastMonth') }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref, watch, onMounted, onUnmounted} from "vue";
import {useI18n} from 'vue-i18n';
import {useSessionStore} from "@/store/session";
import DashboardApi from "@/services/api/dashboard";
import {getColorPair} from "@/utils/color-utils";

const {t} = useI18n({useScope: "local"});
const sessionStore = useSessionStore();
const isLoading = ref(true);
const total = ref(0);
const countByPeriod = ref(0);
const pair = getColorPair(5);
let timer;

onMounted(async () => {
  await getDataAsync(sessionStore.year);
  timer = setInterval(() => getDataAsync(sessionStore.year), 29000 + Math.floor(Math.random() * 1000));
});
onUnmounted(() => clearInterval(timer));
watch(() => sessionStore.year, getDataAsync);

async function getDataAsync(year: number) {
  isLoading.value = true;
  const response = await DashboardApi.getOrdersCount(year);
  if (response.success) {
    total.value = response.data!.countTotal;
    countByPeriod.value = response.data!.countByPeriod;
  }

  isLoading.value = false;

}

</script>

<style scoped lang="scss">

</style>

<i18n locale="en">
{
  "orders": "Orders",
  "ordersInYear": "Orders in {year}",
  "newForLastMonth": "new since last month"
}
</i18n>

<i18n locale="uk">
{
  "orders": "Замовлення",
  "ordersInYear": "Замовлення за {year} р",
  "newForLastMonth": "нових за останній місяць"
}
</i18n>