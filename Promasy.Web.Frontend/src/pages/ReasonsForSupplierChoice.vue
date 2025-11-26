<template>
  <div class="grid">
    <div class="col-12">
      <div class="card">

        <Toolbar class="mb-4">
          <template v-slot:start>
            <div class="my-2">
              <Button :label="t('createDialog.addNew')" icon="pi pi-plus" class="p-button-success mr-2"
                      @click="create"/>
              <Button v-if="isUserAdmin && selectedItems.length > 1" :label="t('merge')" icon="pi pi-angle-double-down"
                      class="p-button-warning mr-2" @click="merge"/>
            </div>
          </template>
        </Toolbar>

        <DataTable ref="dt" :value="items" :lazy="true" :paginator="true"
                   :rows="tableData.offset" :totalRecords="tableData.total" :loading="isLoading"
                   @page="onPageAsync($event)" @sort="onSortAsync($event)"
                   :selectionMode="isUserAdmin ? 'multiple' : 'single'" v-model:selection="selectedItems" dataKey="id"
                   paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                   :rowsPerPageOptions="[10,50,100]"
                   :currentPageReportTemplate="t('table.paginationFooter', { itemName: t('reasonsForSupplierChoice') })"
                   responsiveLayout="scroll">
          <template #header>
            <div class="flex flex-column md:flex-row md:justify-content-between md:align-items-center">
              <h5 class="m-0">{{ t('table.header', {itemName: t('manageReasonsForSupplierChoice')}) }}</h5>
              <span class="block mt-2 md:mt-0 p-input-icon-left">
                  <i class="pi pi-search"/>
                  <InputText v-model.trim="tableData.filter" v-debounce:500="useFilterAsync"
                             :placeholder="t('table.search')"/>
              </span>
            </div>
          </template>

          <Column v-if="isUserAdmin" selectionMode="multiple" headerStyle="width: 3em"></Column>
          <Column field="name" :header="t('name')" :sortable="true" headerStyle="width:35%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('name') }}</span>
              {{ slotProps.data.name }}
            </template>
          </Column>
          <Column field="editor" :header="t('table.columns.editor')" headerStyle="width:25%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editor') }}</span>
              <UserChip :user-id="slotProps.data.editorId" :user-name="slotProps.data.editor"/>
            </template>
          </Column>
          <Column field="editedDate" :header="t('table.columns.editDate')" headerStyle="width:25%; min-width:10rem;">
            <template #body="slotProps">
              <span class="p-column-title">{{ t('table.columns.editDate') }}</span>
              {{ d(new Date(slotProps.data.editedDate), 'long') }}
            </template>
          </Column>
          <Column headerStyle="min-width:10rem;">
            <template #body="slotProps">
              <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="edit(slotProps.data)"/>
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning mt-2"
                      @click="confirmDelete(slotProps.data)"/>
            </template>
          </Column>
        </DataTable>


        <Dialog v-model:visible="itemDialog" :style="{width: '450px'}" :header="t('reasonForSupplierChoiceDetails')" :modal="true"
                class="p-fluid">
          <div class="field">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
            <ErrorWrap :errors="v$.name.$errors" :external-errors="externalErrors['Name']">
              <label for="name">{{ t('name') }}</label>
              <InputText id="name" v-model.trim="item.name" required="true" autofocus/>
            </ErrorWrap>
          </div>

          <template #footer>
            <Button :label="t('cancel')" icon="pi pi-times" class="p-button-text" @click="closeItemDialog"/>
            <Button :label="t('save')" icon="pi pi-check" class="p-button-text" @click="saveAsync"/>
          </template>
        </Dialog>

        <Dialog v-model:visible="deleteItemDialog" :style="{width: '450px'}" :header="t('deleteDialog.header')"
                :modal="true">
          <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
              err
            }}
          </Message>
          <div class="flex align-items-center justify-content-center">
            <i class="pi pi-exclamation-triangle mr-3" style="font-size: 2rem"/>
            <span v-if="item">{{ t('deleteDialog.text') }} <b>{{ item.name }}</b>?</span>
          </div>
          <template #footer>
            <Button :label="t('no')" icon="pi pi-times" class="p-button-text" @click="closeDeleteItemDialog"/>
            <Button :label="t('yes')" icon="pi pi-check" class="p-button-text" @click="deleteItemAsync"/>
          </template>
        </Dialog>

        <Dialog v-model:visible="mergeDialog" :style="{width: '450px'}" :header="t('mergeDialog.header')"
                :modal="true">
          <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{err}}</Message>
          <div class="flex flex-column">
            <i class="flex align-items-center justify-content-center pi pi-exclamation-triangle mr-3" style="font-size: 2rem"/>
            <div class="flex align-items-center justify-content-center">{{ t('mergeDialog.text1') }}:</div>
            <ul>
              <li v-for="item in selectedItems"><b>{{ item.name }}</b></li>
            </ul>
            <div class="flex align-items-center justify-content-center mb-3">{{ t('mergeDialog.text2') }}</div>
            <div class="flex align-items-center justify-content-center">
              <Dropdown v-model="item" :options="selectedItems" optionLabel="name"></Dropdown><span class="ml-1">?</span>
            </div>
          </div>
          <template #footer>
            <Button :label="t('no')" icon="pi pi-times" class="p-button-text" @click="closeMergeDialog"/>
            <Button :label="t('yes')" icon="pi pi-check" class="p-button-text" @click="mergeAsync"/>
          </template>
        </Dialog>

      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import processError from "@/utils/error-response-utils";
import { capitalize } from "@/utils/string-utils";
import { useSessionStore } from "@/store/session";
import { ref, reactive, onMounted, computed } from "vue";
import ReasonsForSupplierChoiceApi, { ReasonForSupplierChoice } from "@/services/api/reasons-for-supplier-choice";
import { useToast } from "primevue/usetoast";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { DataTableSortEvent, DataTablePageEvent } from "primevue/datatable";
import ErrorWrap from "../components/ErrorWrap.vue";
import useVuelidate from "@vuelidate/core";
import { required, maxLength } from "@/i18n/validators";
import UserChip from "@/components/UserChip.vue";

const { d, t } = useI18n();
const { isUserAdmin } = useSessionStore();
const toast = useToast();
const route = useRoute();
const Router = useRouter();
const items = ref([] as ReasonForSupplierChoice[]);
const selectedItems = ref([] as ReasonForSupplierChoice[]);
const externalErrors = ref({} as Object<string[]>);
const item = ref({} as ReasonForSupplierChoice);
const itemDialog = ref(false);
const deleteItemDialog = ref(false);
const mergeDialog = ref(false);
const isLoading = ref(false);
const tableData: TablePagingData = reactive({
  page: 1,
  offset: 10,
  filter: undefined,
  orderBy: undefined,
  descending: undefined,
  total: 0,
});

const rules = computed(() => {
  return {
    name: { required, maxLength: maxLength(300) },
  };
});
const v$ = useVuelidate(rules, item, { $lazy: true });


onMounted(async () => {
  await getDataAsync();
  if(route.path.endsWith("new")) {
    create();
  }
});

async function useFilterAsync() {
  await getDataAsync();
}

async function getDataAsync() {
  isLoading.value = true;
  const response = await ReasonsForSupplierChoiceApi.getList(tableData.page, tableData.offset, tableData.filter, tableData.orderBy, tableData.descending);
  if (response.success) {
    items.value = response.data?.collection ?? [] as ReasonForSupplierChoice[];
    tableData.page = response.data?.page ?? tableData.page;
    tableData.total = response.data?.total ?? 0;
    isLoading.value = false;
    return;
  }
  toast.add({ severity: "error", summary: t("toast.error"), detail: t("table.loadError"), life: 3000 });
  isLoading.value = false;
}

async function onPageAsync(event: DataTablePageEvent) {
  tableData.page = event.page + 1;
  tableData.offset = event.rows;
  await getDataAsync();
}

async function onSortAsync(event: DataTableSortEvent) {
  tableData.orderBy = capitalize(event.sortField as string);
  tableData.descending = event.sortOrder === 1 ? undefined : true;
  await getDataAsync();
}

function create() {
  externalErrors.value = {} as Object<string[]>;
  item.value = {} as ReasonForSupplierChoice;
  v$.value.$reset();
  itemDialog.value = true;
}

function edit(selectedItem: ReasonForSupplierChoice) {
  externalErrors.value = {} as Object<string[]>;
  item.value = { ...selectedItem };
  v$.value.$reset();
  itemDialog.value = true;
}

function merge() {
  externalErrors.value = {} as Object<string[]>;
  item.value = { ...selectedItems.value[0] };
  mergeDialog.value = true;
}

function closeItemDialog() {
  itemDialog.value = false;
}

function closeMergeDialog() {
  mergeDialog.value = false;
}

function closeDeleteItemDialog() {
  deleteItemDialog.value = false;
}

function confirmDelete(selectedItem: ReasonForSupplierChoice) {
  externalErrors.value = {} as Object<string[]>;
  item.value = selectedItem;
  deleteItemDialog.value = true;
}

async function saveAsync() {
  externalErrors.value = {} as Object<string[]>;
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) {
    return;
  }
  const response = await (item.value.id
      ? ReasonsForSupplierChoiceApi.update({ id: item.value.id, name: item.value.name })
      : ReasonsForSupplierChoiceApi.create({ name: item.value.name }));
  if (response.success) {
    itemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
    return;
  }
  if (response.error?.errors) {
    externalErrors.value = response.error.errors;
  }
}

async function deleteItemAsync() {
  externalErrors.value = {} as Object<string[]>;
  const response = await ReasonsForSupplierChoiceApi.delete(item.value.id);
  if (response.success) {
    item.value = {} as ReasonForSupplierChoice;
    deleteItemDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
  } else {
    processError(response.error, (errs) => { externalErrors.value = errs });
  }
}

async function mergeAsync() {
  externalErrors.value = {} as Object<string[]>;
  const response = await ReasonsForSupplierChoiceApi.merge({sourceIds: selectedItems.value.filter(i => i.id !== item.value.id).map(i => i.id), targetId: item.value.id});
  if (response.success) {
    if(route.path.endsWith("new")) {
      await Router.push({ name: "ReasonsForSupplierChoice" });
      return;
    }
    item.value = {} as ReasonForSupplierChoice;
    selectedItems.value = [];
    mergeDialog.value = false;
    await getDataAsync();
    toast.add({ severity: "success", summary: t("toast.success"), life: 3000 });
  } else {
    processError(response.error, (errs) => { externalErrors.value = errs });
  }
}
</script>

<i18n locale="en">
{
  "reasonsForSupplierChoice": "reasons for supplier choice",
  "manageReasonsForSupplierChoice": "reasons for supplier choice",
  "name": "Name",
  "reasonForSupplierChoiceDetails": "Reason for supplier choice details"
}
</i18n>

<i18n locale="uk">
{
  "reasonsForSupplierChoice": "причин вибору постачальника",
  "manageReasonsForSupplierChoice": "причинами вибору постачальника",
  "name": "Назва",
  "reasonForSupplierChoiceDetails": "Деталі причини вибору постачальника"
}
</i18n>