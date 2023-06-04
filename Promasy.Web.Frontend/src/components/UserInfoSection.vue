<template>
  <div>
    <div v-if="config.name" class="text-900 font-medium text-xl mb-2">{{ user.firstName }} {{ user.middleName }} {{user.lastName}} </div>
    <div v-if="config.role" class="text-600">
      <RoleBadge v-for="role in user?.roles" :key="role" :role="role" v-tooltip.left="t('role')"></RoleBadge>
    </div>
    <hr class="my-3 mx-0 border-top-1 border-none surface-border"/>
    <ul class="list-none p-0 m-0 flex-grow-1">
      <li v-if="config.organization" class="flex align-items-center mb-3">
        <i class="pi pi-building text-green-500 mr-2"></i>
        <span v-tooltip.left="t('organization')">{{ user.organization }}</span>
      </li>
      <li v-if="config.department" class="flex align-items-center mb-3">
        <i class="pi pi-sitemap text-green-500 mr-2"></i>
        <span v-tooltip.left="t('department')">{{ user.department }}</span>
      </li>
      <li v-if="config.subDepartment && user.subDepartment !== EmptySubDepartmentName" class="flex align-items-center mb-3">
        <i class="pi pi-briefcase text-green-500 mr-2"></i>
        <span v-tooltip.left="t('sub-department')">{{ user.subDepartment }}</span>
      </li>
      <li v-if="config.phone" class="flex align-items-center mb-3">
        <i class="pi pi-phone text-green-500 mr-2"></i>
        <span v-tooltip.left="t('phone')"><a :href="`tel:${user.primaryPhone}`">{{user.primaryPhone}}</a></span>
      </li>
      <li v-if="config.phone && user.reservePhone" class="flex align-items-center mb-3">
        <i class="pi pi-phone text-green-500 mr-2"></i>
        <span v-tooltip.left="t('phone')"><a :href="`tel:${user.reservePhone}`">{{user.reservePhone}}</a></span>
      </li>
      <li v-if="config.email" class="flex align-items-center mb-3">
        <i class="pi pi-at text-green-500 mr-2"></i>
        <span v-tooltip.left="t('email')"><a :href="`mailto:${user.email}`">{{user.email}}</a></span>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from "vue";
import {Employee} from "@/services/api/employees";
import RoleBadge from "@/components/RoleBadge.vue";
import {useI18n} from 'vue-i18n';
import {EmptySubDepartmentName} from "@/services/api/sub-departments";

const {t} = useI18n({useScope: "local"});
const props = defineProps<{
  user: Employee,
  config: UserInfoSectionConfig
}>()


export interface UserInfoSectionConfig {
  name: boolean;
  role: boolean;
  organization: boolean;
  department: boolean;
  subDepartment: boolean;
  phone: boolean;
  email: boolean;
}
</script>

<style scoped lang="scss">

</style>

<i18n locale="en">
{
  "organization": "Organization",
  "role": "Role",
  "phone": "Phone",
  "email": "Email"
}
</i18n>

<i18n locale="uk">
{
  "organization": "Організація",
  "role": "Роль",
  "phone": "Телефон",
  "email": "Електронна пошта"
}
</i18n>