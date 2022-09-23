<template>
  <TreeTable :value="nodes" :lazy="true" :loading="loading" @nodeExpand="onExpand">
    <template #header>
      <div class="text-right">
        <template v-if="nodes.length && nodes[0].data.level > 1">
          <Button icon="pi pi-home" class="mr-2 p-button-outlined p-button-secondary"
                  v-tooltip.left="t('backToCpvBeginning')" @click="backToRootAsync"/>
          <Button icon="pi pi-arrow-up" class="mr-2 p-button-outlined p-button-secondary"
                  v-tooltip.left="t('levelUp')"
                  @click="backOneLevelAsync"/>
        </template>
        <div class="p-input-icon-left">
          <i class="pi pi-search"></i>
          <InputText v-model.trim="filter" v-debounce:500="useFilter" :placeholder="t('table.search')"
                     size="50"/>
        </div>
      </div>
    </template>
    <Column field="code" :header="t('code')" :expander="true"></Column>
    <Column field="descriptionUkrainian" :header="t('descriptionUkrainian')"></Column>
    <Column field="descriptionEnglish" :header="t('descriptionEnglish')"></Column>
    <Column v-if="props.selectMode" headerStyle="width:5rem;">
      <template #body="slotProps">
        <Button v-if="isSelectable(slotProps.node.data)"
                icon="pi pi-check"
                class="p-button-rounded p-button-sm p-button-success"
                :class="{'p-button-outlined': slotProps.node.data.id !== preSelectedValue?.id }"
                @click="onSelectItem(slotProps.node.data)"
                v-tooltip.left="t('select')"/>
      </template>
    </Column>
  </TreeTable>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import CpvApi, { Cpv } from "@/services/api/cpv";
import { TreeNode } from "primevue/tree";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  selectMode: boolean,
  modelValue: Cpv | undefined,
}>();

const emit = defineEmits([ "loading", "update:modelValue" ]);

const preSelectedValue = ref(undefined as Cpv | undefined);

const { t } = useI18n({ useScope: "local" });
const nodes = ref([] as TreeNode[]);
const loading = ref(false);
const filter = ref("");

onMounted(async () => {
  await getDataAsync(undefined, props.modelValue?.id, undefined,
      (cpvs: Cpv[]) => {
        if (props.modelValue) {
          preSelectedValue.value = cpvs.find(c => c.id === props.modelValue!.id);
        }
      });
});

const onExpand = async (node: TreeNode) => {
  if (!node.leaf) {
    loading.value = true;
    const response = await CpvApi.getList(node.data.id);
    if (response.success) {
      node.children = mapToTreeNode(response.data!);
    }
    loading.value = false;
  }
};

function mapToTreeNode(data: Cpv[]) {
  return data!.map((c): TreeNode => {
    return { key: c.code, data: c, leaf: c.isTerminal };
  });
}

async function useFilter(text: string) {
  if (text && text.length === 1 && !parseInt(text)) {
    return;
  }
  await getDataAsync(undefined, undefined, text);
}

async function backToRootAsync() {
  filter.value = "";
  await getDataAsync();
}

async function backOneLevelAsync() {
  filter.value = "";
  const childNodes = nodes.value;
  const node = childNodes[0];
  loading.value = true;
  const responseParentItem = await CpvApi.getList(undefined, node.data.parentId);
  if (responseParentItem.success) {
    const response = await CpvApi.getList(node.data.level > 2 ? responseParentItem.data![0].parentId : undefined);
    if (response.success) {
      nodes.value = mapToTreeNode(response.data!);
      const parentNode = nodes.value.find(n => n.data.id === node.data.parentId)!;
      parentNode.children = childNodes;
    }
  }
  loading.value = false;
}

async function getDataAsync(parentId?: number, id?: number, searchText?: string, onSuccess?: (cpvs: Cpv[]) => void) {
  loading.value = true;
  const response = await CpvApi.getList(parentId, id, searchText);
  if (response.success) {
    nodes.value = mapToTreeNode(response.data!);
    if (onSuccess) {
      onSuccess(response.data!);
    }
  }
  loading.value = false;
}

function isSelectable(cpv: Cpv | undefined): boolean {
  return !!cpv && (cpv.isTerminal || cpv.level > 3);
}

function onSelectItem(cpv: Cpv) {
  preSelectedValue.value = cpv;
  emit("update:modelValue", cpv);
}
</script>

<i18n locale="en">
{
  "code": "Code",
  "select": "Select",
  "backToCpvBeginning": "Back to CPV root",
  "levelUp": "Back one CPV level",
  "descriptionUkrainian": "Description in Ukrainian",
  "descriptionEnglish": "Description in English"
}
</i18n>

<i18n locale="uk">
{
  "code": "Код",
  "select": "Обрати",
  "backToCpvBeginning": "Повернутися до початкових категорій",
  "levelUp": "Повернутися на категорію вище",
  "descriptionUkrainian": "Опис українськю",
  "descriptionEnglish": "Опис англійською"
}
</i18n>