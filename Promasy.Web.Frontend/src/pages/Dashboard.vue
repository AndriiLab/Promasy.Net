<template>
  <div class="grid grid-cols-12 gap-8">
    <div class="col-span-12 lg:col-span-6 xl:col-span-3">
      <OrdersCountDashboard />
    </div>
    <div class="col-span-12 lg:col-span-6 xl:col-span-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Material" :icon="'pi-map-marker'" />
    </div>
    <div class="col-span-12 lg:col-span-6 xl:col-span-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Equipment" :icon="'pi-inbox'" />
    </div>
    <div class="col-span-12 lg:col-span-6 xl:col-span-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Service" :icon="'pi-comment'" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import OrdersCountDashboard from "@/components/OrdersCountDashboard.vue";
import FinancesLeftDashboard from "@/components/FinancesLeftDashboard.vue";
import {OrderTypeEnum} from "@/constants/OrderTypeEnum";
</script>

