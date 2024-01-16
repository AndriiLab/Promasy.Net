<template>
  <div style="display: inline">
    <Avatar
        class="user-avatar"
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
import {watch, ref} from "vue";
import {useI18n} from 'vue-i18n';
import UserInfoSection, {UserInfoSectionConfig} from "@/components/UserInfoSection.vue";
import EmployeesApi, {Employee} from "@/services/api/employees";
import {getColorPair} from "@/utils/color-utils";

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
let pair = getColorPair(props.userId);
const backgroundColor = ref(pair.backgroundColor);
const color = ref(pair.foregroundColor);

watch(() => props.userName, (val) => {
  initials.value = getInitials(val);
});

watch(() => props.userId, (val) => {
  pair = getColorPair(props.userId);
  backgroundColor.value = pair.backgroundColor;
  color.value = pair.foregroundColor
});

function getInitials(name: string) {
  {
    const arr = name.split(' ');
    if (arr.length < 2) {
      return arr[0].charAt(0);
    }

    return `${arr[1].charAt(0)}${arr[0].charAt(0)}`;
  }
}

async function togglePopupAsync(ev: Event, loadData: boolean) {
  userChipDetailsPanel.value?.toggle(ev);
  if (!loadData) {
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

<style lang="scss">
.user-avatar {
  cursor: pointer;
  -webkit-user-select: none;
  -ms-user-select: none;
  user-select: none;
}
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