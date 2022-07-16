<template>
  <Dropdown  :options="options" optionLabel="text"
            :filter="true" v-on:before-show="getListAsync"
            v-model="value"
            optionValue="value" :loading="isLoading" :disabled="disabled ?? false"></Dropdown>
</template>

<script lang="ts" setup>
import SubDepartmentsApi from "@/services/api/sub-departments";
import { useSessionStore } from "@/store/session";
import { SelectItem } from "@/utils/fetch-utils";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  departmentId: number,
  modelValue: number,
  includeEmpty: boolean,
  emptyText?: string,
  defaultOptions: SelectItem<number>[]
  disabled?: boolean,
}>();

const emit = defineEmits([ "loading", "update:modelValue", "update:selectedObject" ]);

const { t } = useI18n();
const { user } = useSessionStore();

const options = ref([] as SelectItem<number>[]);
const isLoading = ref(false);

onMounted(() => {
  options.value.push(...props.defaultOptions);
  if(props.includeEmpty) {
    options.value.push(getDefaultItem());
  }
});

watch(() => props.defaultOptions, (o) => options.value.push(...o));
watch(() => props.departmentId, (id, oldId) => {
  if (id !== oldId) {
    options.value = [];
    if(props.includeEmpty) {
      options.value.push(getDefaultItem());
    }
    value.value = 0;
  }
});

const value = computed({
  get() {
    return props.modelValue;
  },
  set(value) {
    emit("update:modelValue", value);
    emit("update:selectedObject", { ...options.value.find(o => o.value === value) });
  },
});

async function getListAsync() {
  isLoading.value = true;
  emit("loading", isLoading.value);
  const response = await SubDepartmentsApi.getList(user!.organizationId, props.departmentId, 1, 1000, undefined, "Name", undefined);

  if (response) {
    options.value = [];
    options.value.push(getDefaultItem());
    response.data!.collection.map(d => {
      return { value: d.id, text: d.name } as SelectItem<number>;
    }).forEach(i => options.value.push(i));
  }
  isLoading.value = false;
  emit("loading", isLoading.value);
}

function getDefaultItem(): SelectItem<number> {
  return { value: 0, text: props.emptyText ?? t("all") };
}
</script>

<i18n locale="en">
{
  "all": "-all sub-departments-"
}
</i18n>

<i18n locale="uk">
{
  "all": "-всі підрозділи-"
}
</i18n>