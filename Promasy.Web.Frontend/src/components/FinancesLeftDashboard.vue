<template>
  <div class="card mb-0">
    <div class="flex justify-content-between mb-3">
      <div>
        <span class="block text-500 font-medium mb-3">{{
            t(sessionStore.year === new Date().getFullYear() ? 'left' : 'leftInYear', {year: sessionStore.year, type: getOrderTypeName(props.type) })
          }}</span>
        <Skeleton v-if="isLoading" width="5rem" class="mb-2"></Skeleton>
        <div v-else class="text-900 font-medium text-xl">
          {{ currency(total).format() }}
        </div>
      </div>
      <div class="flex align-items-center justify-content-center border-round"
           :style="{ width:'2.5rem', height:'2.5rem', backgroundColor: pair.backgroundColor }">
        <i :class="['pi', props.icon, 'text-xl']" :style="{ color: pair.foregroundColor }"></i>
      </div>
    </div>
    <Skeleton v-if="isLoading" width="2rem" class="mb-2"></Skeleton>
    <div v-else>
      <span class="text-red-500 font-medium mr-1">
      {{ currency(countByPeriod).format() }}
      </span>
      <span class="text-500">{{ t('newForLastMonth') }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref, watch, onMounted, onUnmounted, defineProps} from "vue";
import currency from "@/utils/currency-utils";
import {useI18n} from 'vue-i18n';
import {useSessionStore} from "@/store/session";
import DashboardApi from "@/services/api/dashboard";
import {getColorPair} from "@/utils/color-utils";
import {OrderTypeEnum} from "@/constants/OrderTypeEnum";
import {getOrderTypeName} from "@/constants/OrderTypeEnum";

const {t} = useI18n({useScope: "local"});
const sessionStore = useSessionStore();
const isLoading = ref(true);
const total = ref(0);
const countByPeriod = ref(0);
const props = defineProps<{
  type: OrderTypeEnum,
  icon: string
}>();
const pair = getColorPair(props.type);
let timer;


onMounted(async () => {
  await getDataAsync(sessionStore.year, props.type);
  timer = setInterval(() => getDataAsync(sessionStore.year, props.type), 29000 + Math.floor(Math.random() * 1000));
});
onUnmounted(() => clearInterval(timer));
watch(() => sessionStore.year, (y) => getDataAsync(y, props.type));

async function getDataAsync(year: number, type: OrderTypeEnum) {
  isLoading.value = true;
  const response = await DashboardApi.getFundingLeft(year, type);
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
  "left": "{type}",
  "leftInYear": "{type} in {year}",
  "newForLastMonth": "since last month"
}
</i18n>

<i18n locale="uk">
{
  "left": "{type}. Залишок",
  "leftInYear": "{type}. Залишок за {year} р",
  "newForLastMonth": "за останній місяць"
}
</i18n>