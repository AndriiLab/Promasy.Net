<template>
  <div class="grid">
    <div class="col-12 lg:col-6 xl:col-3">
      <OrdersCountDashboard />
    </div>
    <div class="col-12 lg:col-6 xl:col-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Material" :icon="'pi-map-marker'" />
    </div>
    <div class="col-12 lg:col-6 xl:col-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Equipment" :icon="'pi-inbox'" />
    </div>
    <div class="col-12 lg:col-6 xl:col-3">
      <FinancesLeftDashboard :type="OrderTypeEnum.Service" :icon="'pi-comment'" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import OrdersCountDashboard from "@/components/OrdersCountDashboard.vue";
import FinancesLeftDashboard from "@/components/FinancesLeftDashboard.vue";
import {OrderTypeEnum} from "@/constants/OrderTypeEnum";
</script>