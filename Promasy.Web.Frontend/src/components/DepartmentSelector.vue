<template>
  <div>
    <label for="department" :class="labelClasses">{{ t('department') }}</label>
    <Dropdown id="department" :class="selectorClasses" :options="options" optionLabel="text"
              :filter="true" v-on:before-show="getListAsync"
              v-model="value"
              optionValue="value" :loading="isLoading" :disabled="disabled ?? false"></Dropdown>
  </div>
</template>

<script lang="ts" setup>
import DepartmentsApi from "@/services/api/departments";
import { useSessionStore } from "@/store/session";
import { SelectItem } from "@/utils/fetch-utils";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue: number,
  defaultOptions: SelectItem<number>[]
  includeEmpty: boolean,
  disabled?: boolean,
  labelClasses?: string[],
  selectorClasses?: string[],
}>();

const emit = defineEmits([ "loading", "update:modelValue", "update:selectedObject" ]);

const { t } = useI18n();
const { user } = useSessionStore();

const options = ref([] as SelectItem<number>[]);
const isLoading = ref(false);

onMounted(() => {
  options.value.push(...props.defaultOptions);
});

watch(() => props.defaultOptions, (o) => options.value.push(...o));

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
  const response = await DepartmentsApi.getList(user!.organizationId, 1, 1000, undefined, "Name", undefined);

  if (response) {
    options.value = [];
    if (props.includeEmpty) {
      options.value.push(getDefaultItem());
    }
    response.data!.collection.map(d => {
      return { value: d.id, text: d.name } as SelectItem<number>;
    }).forEach(i => options.value.push(i));
  }
  isLoading.value = false;
  emit("loading", isLoading.value);
}

function getDefaultItem(): SelectItem<number> {
  return { value: 0, text: t("all") };
}
</script>

<i18n locale="en">
{
  "department": "Department",
  "all": "-all departments-"
}
</i18n>

<i18n locale="uk">
{
  "department": "Відділ",
  "all": "-всі відділи-"
}
</i18n>