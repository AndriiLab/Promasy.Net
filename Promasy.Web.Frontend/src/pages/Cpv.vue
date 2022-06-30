<template>
	<div class="grid">
		<div class="col-12">
			<div class="card">
				<TreeTable :value="nodes" :lazy="true" :loading="loading" @nodeExpand="onExpand">
					<template #header>
						<div class="text-right">
							<div class="p-input-icon-left">
								<i class="pi pi-search"></i>
								<InputText v-model.trim="filter" :placeholder="t('search')"
									size="50" />
							</div>
						</div>
					</template>
					<Column field="code" :header="t('code')" :expander="true"></Column>
					<Column field="descriptionUkrainian" :header="t('descriptionUkrainian')"></Column>
					<Column field="descriptionEnglish" :header="t('descriptionEnglish')"></Column>
				</TreeTable>
			</div>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { ref, onMounted, watch } from 'vue';
import CpvApi, { Cpv } from '@/services/api/cpv';
import { TreeNode } from "primevue/tree";
import { useI18n } from 'vue-i18n';

const { t } = useI18n({ useScope: "local" });
const nodes = ref([] as TreeNode[]);
const loading = ref(false);
const filter = ref('');

onMounted(async () => {
	loading.value = true;

	const response = await CpvApi.getList();
	if (response.success) {
		nodes.value = mapToTreeNode(response.data!);
	}
	loading.value = false;
})

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
	return data!.map((c): TreeNode => { return { key: c.code, data: c, leaf: c.isTerminal } });
}


watch(filter, async (text) => {
	if (text && text.length === 1 && !parseInt(text)) {
		return;
	}
	loading.value = true;
	const response = await CpvApi.getList(undefined, text);
	if (response.success) {
		nodes.value = mapToTreeNode(response.data!);
	}
	loading.value = false;
});
</script>

<i18n locale="en">
{
    "search": "Search",
    "code": "Code",
    "descriptionUkrainian": "Description in Ukrainian",
    "descriptionEnglish": "Description in English",
}
</i18n>

<i18n locale="uk">
{
    "search": "Пошук",
    "code": "Код",
    "descriptionUkrainian": "Опис українськю",
    "descriptionEnglish": "Опис англійською",
}
</i18n>