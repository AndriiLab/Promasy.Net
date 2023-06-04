<template>
  <div style="display: inline">
    <Avatar
        :label="initials"
        :style="{ 'background-color': backgroundColor, color: color }"
        @click="togglePopupAsync($event, true)"/>
    <OverlayPanel ref="userChipDetailsPanel" @mouseleave="togglePopupAsync($event, false)">
      <div v-if="!showDetails">
        <p class="pl-2">{{ userName }}</p>
      </div>
      <UserInfoSection v-else :user="user" :config="userInfoConfig"/>
    </OverlayPanel>
  </div>
</template>

<script setup lang="ts">
import {watch, ref, defineProps} from "vue";
import tinycolor from "tinycolor2";
import {useI18n} from 'vue-i18n';
import UserInfoSection, {UserInfoSectionConfig} from "@/components/UserInfoSection.vue";
import EmployeesApi, {Employee} from "@/services/api/employees";

const {t} = useI18n({useScope: "local"});
const userChipDetailsPanel = ref(null);
const showDetails = ref(false);
const user = ref({} as Employee);
const userInfoConfig = {
  name: true,
  role: true,
  department: true,
  subDepartment: true,
  phone: true,
  email: true
} as UserInfoSectionConfig;

const props = defineProps<{
  userName: string,
  userId: number
}>();

const initials = ref(getInitials(props.userName));
const backgroundColor = ref(generateBackgroundColor(props.userId));
const color = ref(generateForegroundColor(backgroundColor.value));

watch(() => props.userName, (val) => {
  initials.value = getInitials(val);
});

watch(() => props.userId, (val) => {
  backgroundColor.value = generateBackgroundColor(val);
  color.value = generateForegroundColor(backgroundColor.value)
});

function generateBackgroundColor(num: number) {
  return `hsl(${num * 137.508},50%,75%)`;
}

function getInitials(name: string) {
  {
    const arr = name.split(' ');
    if (arr.length < 2) {
      return arr[0].charAt(0);
    }

    return `${arr[1].charAt(0)}${arr[0].charAt(0)}`;
  }
}

function generateForegroundColor(bgColor: string) {
  return tinycolor(bgColor).getLuminance() > 0.179 ? '#000' : '#fff';
}

async function togglePopupAsync(ev: Event, loadData: boolean) {
  userChipDetailsPanel.value?.toggle(ev);
  if(!loadData) {
    showDetails.value = false;
    return;
  }

  const response = await EmployeesApi.getById(props.userId);
  if (!response.success) {
    showDetails.value = false;
    return;
  }
  user.value = response.data!;
  showDetails.value = true;
}

</script>

<style scoped lang="scss">

</style>

<i18n locale="en">
{
  "moreDetails": "More details"
}
</i18n>

<i18n locale="uk">
{
  "moreDetails": "Більше інформації"
}
</i18n>