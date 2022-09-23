<template>
  <Loader v-if="initializing"></Loader>

  <div v-else class="grid">
    <div class="col-12">
      <div class="card">
        <div class="p-fluid formgrid grid">
          <div class="field col-12">
            <h3>{{ header }}</h3>
          </div>

          <div class="field col-12">
            <h4>{{ t('funding') }}</h4>
          </div>

          <ErrorWrap :errors="v$.departmentId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['DepartmentId']">
            <label for="department">{{ t('department') }}</label>
            <DepartmentSelector id="department"
                                :default-options="departments"
                                v-model="model.departmentId"
                                :include-empty="false">
            </DepartmentSelector>
          </ErrorWrap>
          <ErrorWrap :errors="v$.subDepartmentId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['SubDepartmentId']">
            <label for="subDepartment">{{ t('sub-department') }}</label>
            <SubDepartmentSelector id="subDepartment"
                                   :department-id="model.departmentId"
                                   :default-options="subDepartments"
                                   v-model="model.subDepartmentId"
                                   :include-empty="false">
            </SubDepartmentSelector>
          </ErrorWrap>

          <ErrorWrap :errors="v$.type.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['Type']">
            <label for="type">{{ t('type') }}</label>
            <Dropdown id="type" v-model="model.type" :options="types"
                      optionLabel="text" optionValue="value"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.financeSubDepartmentId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['FinanceSubDepartmentId']">
            <label for="financeSubDepartment">{{ t('financeSource') }}</label>
            <Dropdown id="financeSubDepartment"
                      :modelValue="selectedFinanceSubDepartment" v-on:update:modelValue="onFinanceSubDepartmentSelected"
                      :options="financeSubDepartments"
                      v-on:before-show="getFinanceSubDepartmentsListAsync"
                      :filter="true"
                      :optionLabel="getFinanceLabel" :loading="loading"/>
          </ErrorWrap>

          <div class="field col-12">
            <h4>{{ t('order') }}</h4>
          </div>
          <ErrorWrap :errors="v$.manufacturerId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['ManufacturerId']">
            <label for="manufacturerId">{{ t('manufacturer') }}</label>
            <div class="p-inputgroup">
              <Dropdown id="manufacturerId" v-model="model.manufacturerId"
                        :options="manufacturers"
                        v-on:before-show="getManufacturersAsync"
                        :filter="true"
                        optionLabel="text" optionValue="value" :loading="loading"/>
              <router-link :to="{ name: 'CreateManufacturer'}" target="_blank">
                <Button icon="pi pi-plus" class="p-button-outlined p-button-success"
                        v-tooltip.bottom="t('createDialog.addNew')"/>
              </router-link>
            </div>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-6" :errors="v$.catNum.$errors"
                     :external-errors="externalErrors['CatNum']">
            <label for="catNum">{{ t('catNum') }}</label>
            <InputText id="catNum" v-model="model.catNum" :disabled="!isManufacturerSelected"
                       :placeholder="isManufacturerSelected ? '' : t('selectManufacturerFirst')"/>
          </ErrorWrap>

          <ErrorWrap class="field col-12" :errors="v$.description.$errors"
                     :external-errors="externalErrors['Description']">
            <label for="description">{{ t('description') }}</label>
            <Textarea id="description" v-model.trim="model.description" :autoResize="true" class="w-full"/>
          </ErrorWrap>
          <ErrorWrap class="field col-12" :errors="v$.cpvId.$errors" :external-errors="externalErrors['CpvId']">
            <label for="cpv">CPV</label>
            <div class="p-inputgroup">
              <InputText id="cpv"
                         :modelValue="selectedCpv ? `${selectedCpv.code} - ${selectedCpv.descriptionUkrainian} - ${selectedCpv.descriptionEnglish}` : ''"
                         disabled/>
              <Button icon="pi pi-search" class="p-button-outlined p-button-info" @click="selectCpvDialog = true"/>
            </div>
          </ErrorWrap>

          <div class="field col-12">
            <h4>{{ t('supplier') }}</h4>
          </div>
          <ErrorWrap :errors="v$.supplierId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['SupplierId']">
            <label for="supplierId">{{ t('supplierName') }}</label>
            <div class="p-inputgroup">
              <Dropdown id="supplierId" v-model="model.supplierId"
                        :options="suppliers"
                        v-on:before-show="getSuppliersAsync"
                        :filter="true"
                        optionLabel="text" optionValue="value" :loading="loading"/>
              <router-link :to="{ name: 'CreateSupplier'}" target="_blank">
                <Button icon="pi pi-plus" class="p-button-outlined p-button-success"
                        v-tooltip.bottom="t('createDialog.addNew')"/>
              </router-link>
            </div>
          </ErrorWrap>
          <ErrorWrap :errors="v$.reasonId.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['ReasonId']">
            <label for="reasonId">{{ t('reasonForSupplierChoice') }}</label>
            <div class="p-inputgroup">
              <Dropdown id="reasonId" v-model="model.reasonId"
                        :options="reasons"
                        v-on:before-show="getReasonsForSupplierChoiceAsync"
                        :filter="true"
                        :disabled="!isSupplierSelected"
                        :placeholder="isSupplierSelected ? '' : t('selectSupplierFirst')"
                        optionLabel="text" optionValue="value" :loading="loading"/>
              <router-link :to="{ name: 'CreateReasonForSupplierChoice'}" target="_blank">
                <Button icon="pi pi-plus" class="p-button-outlined p-button-success"
                        v-tooltip.bottom="t('createDialog.addNew')"/>
              </router-link>
            </div>
          </ErrorWrap>

          <div class="field col-12">
            <h4>{{ t('price') }}</h4>
          </div>
          <ErrorWrap class="field col-12 md:col-3" :errors="v$.onePrice.$errors"
                     :external-errors="externalErrors['OnePrice']">
            <label for="onePrice">{{ t('priceOfOneUnit') }}</label>
            <FinanceInput input-id="onePrice" v-model="model.onePrice" input-placeholder="">
              <template v-slot:suffix>
                <span class="p-inputgroup-addon">
                  <Checkbox v-model="model.taxIncluded" :binary="true"/>
                  <span class="ml-1">{{ t('taxIncluded') }}</span>
                </span>
              </template>
            </FinanceInput>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-3" :errors="v$.amount.$errors"
                     :external-errors="externalErrors['Amount']">
            <label for="amount">{{ t('amount') }}</label>
            <InputText id="onePrice" v-model.number="model.amount" min="0" step="1"/>
          </ErrorWrap>
          <ErrorWrap :errors="v$.unitId.$errors" class="field col-12 md:col-3"
                     :external-errors="externalErrors['UnitId']">
            <label for="unitId">{{ t('units') }}</label>
            <div class="p-inputgroup">
              <Dropdown id="unitId" v-model="model.unitId"
                        :options="units"
                        v-on:before-show="getUnitsAsync"
                        :filter="true"
                        optionLabel="text" optionValue="value" :loading="loading"/>
              <router-link :to="{ name: 'CreateUnit'}" target="_blank">
                <Button icon="pi pi-plus" class="p-button-outlined p-button-success"
                        v-tooltip.bottom="t('createDialog.addNew')"/>
              </router-link>
            </div>
          </ErrorWrap>
          <div class="field col-12 md:col-3">
            <label></label>
            <div class="mt-3">
              <h5><span>{{ t('total') }}: </span><span>{{ currency(total).format() }}</span></h5>
            </div>
          </div>

          <div class="field col-12">
            <h4>{{ t('procurement') }}</h4>
          </div>
          <ErrorWrap :errors="v$.procurementStartDate.$errors" class="field col-12 md:col-6"
                     :external-errors="externalErrors['ProcurementStartDate']">
            <label for="procurementStartDate">{{ t('procurementStartDate') }}</label>
            <Calendar id="procurementStartDate" v-model="model.procurementStartDate"></Calendar>
          </ErrorWrap>
          <ErrorWrap class="field col-12 md:col-6" :errors="v$.kekv.$errors"
                     :external-errors="externalErrors['Kekv']">
            <label for="kekv" v-tooltip.bottom="t('kekvFull')">{{ t('kekv') }}</label>
            <div class="p-inputgroup">
              <InputText id="kekv" v-model="model.kekv" :disabled="!isKekvEdit"/>
              <Button icon="pi pi-pencil" class="p-button-outlined p-button-success"
                      v-on:click="isKekvEdit = !isKekvEdit"/>
            </div>
          </ErrorWrap>

          <div class="field col-12">
            <Message v-for="err of externalErrors['']" :severity="'error'" :key="err" :closable="false">{{
                err
              }}
            </Message>
          </div>
        </div>

        <div class="flex justify-content-between flex-wrap mt-5">
          <div v-if="model.editedDate">{{ t('lastEdit') }}:
            <Chip :label="model.editor" icon="pi pi-user"/>
            {{ d(new Date(model.editedDate), 'long') }}
          </div>
          <Button :label="t('save')" icon="pi pi-check" class="p-button" @click="saveAsync"/>
        </div>
      </div>
    </div>

    <Dialog v-model:visible="selectCpvDialog" :header="t('selectCpv')" :modal="true" style="z-index: 999">
      <template v-if="selectCpvDialog">
        <CpvSelector :select-mode="true" :modelValue="selectedCpv" v-on:update:modelValue="onCpvSelected"></CpvSelector>
      </template>
    </Dialog>

  </div>
</template>

<script lang="ts" setup>
import { Cpv } from "@/services/api/cpv";
import OrderApi, { CreateOrderRequest, Order, OrderType, UpdateOrderRequest } from "@/services/api/orders";
import FinanceSubDepartmentsApi, { FinanceSubDepartment } from "@/services/api/finance-sub-departments";
import ManufacturersApi from "@/services/api/manufacturers";
import SuppliersApi from "@/services/api/suppliers";
import ReasonsForSupplierChoiceApi from "@/services/api/reasons-for-supplier-choice";
import UnitsApi from "@/services/api/units";
import { useSessionStore } from "@/store/session";
import { formatAsDate } from "@/utils/date-utils";
import { SelectItem } from "@/utils/fetch-utils";
import { ref, onMounted, computed, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import useVuelidate from "@vuelidate/core";
import currency from "@/utils/currency-utils";
import { minLength, maxLength, required, requiredIf } from "@/i18n/validators";
import Loader from "@/components/Loader.vue";
import DepartmentSelector from "@/components/DepartmentSelector.vue";
import SubDepartmentSelector from "@/components/SubDepartmentSelector.vue";
import ErrorWrap from "@/components/ErrorWrap.vue";
import FinanceInput from "@/components/FinanceInput.vue";
import CpvSelector from "@/components/CpvSelector.vue";

const { t, d } = useI18n({ useScope: "local" });
const Router = useRouter();
const route = useRoute();
const session = useSessionStore();
const user = session.user!;

const header = ref("");
const loading = ref(false);
const initializing = ref(true);
const selectCpvDialog = ref(false);
const externalErrors = ref({} as Object<string[]>);
const model = ref(getDefaultModel());
const departments = ref([] as SelectItem<number>[]);
const subDepartments = ref([] as SelectItem<number>[]);
const financeSubDepartments = ref([] as FinanceSubDepartment[]);
const selectedFinanceSubDepartment = ref({} as FinanceSubDepartment | undefined);
const selectedCpv = ref(undefined as Cpv | undefined);
const types = ref([] as SelectItem<number>[]);
const units = ref([] as SelectItem<number>[]);
const manufacturers = ref([ getDefaultSelectItem() ] as SelectItem<number>[]);
const suppliers = ref([ getDefaultSelectItem() ] as SelectItem<number>[]);
const reasons = ref([] as SelectItem<number>[]);
const isKekvEdit = ref(false);

const greaterThanZero = (v: number) => v > 0;
const rules = computed(() => {
  return {
    departmentId: { required },
    subDepartmentId: { required },
    financeSubDepartmentId: { required },
    amount: { required: greaterThanZero },
    onePrice: { required: greaterThanZero },
    catNum: { requiredIf: requiredIf(model.value.manufacturerId > 0), maxLength: maxLength(300) },
    cpvId: { required },
    description: { required, minLength: minLength(3), maxLength: maxLength(1000) },
    kekv: { maxLength: maxLength(100) },
    manufacturerId: {},
    procurementStartDate: {},
    supplierId: {},
    reasonId: { requiredIf: requiredIf(model.value.supplierId > 0) },
    type: { required },
    unitId: { required },
  };
});
const isManufacturerSelected = computed(() => !!model.value.manufacturerId && model.value.manufacturerId > 0);
const isSupplierSelected = computed(() => !!model.value.supplierId && model.value.supplierId > 0);
const total = computed(() => {
  const t = model.value.amount * model.value.onePrice * (model.value.taxIncluded ? 1 : 1.2);
  return t > 0 ? t : 0;
});
const v$ = useVuelidate(rules, model, { $lazy: true });

onMounted(async () => {
  loading.value = true;
  let loaded = false;
  const orderTypes = await OrderApi.getExistingOrderTypes();
  if (orderTypes.success) {
    types.value = orderTypes.data!;
  }
  if (route.path.endsWith("new")) {
    header.value = t("createOrder");
    model.value = getDefaultModel();
    departments.value = [ { text: user.department, value: user.departmentId } ];
    subDepartments.value = [ { text: user.subDepartment, value: user.subDepartmentId } ];
    loaded = true;
  } else {
    header.value = t("editOrder");
    const orderId = parseInt(route.params.orderId?.toString());
    const response = await OrderApi.getById(orderId);
    if (response.success) {
      const data = response.data!;
      model.value = mapToModel(data);
      departments.value = [ { text: data.department.name, value: data.department.id } ];
      subDepartments.value = [ { text: data.subDepartment.name, value: data.subDepartment.id } ];
      const fs = data.financeSubDepartment as FinanceSubDepartment;
      fs.subDepartmentId = data.subDepartment.id;
      financeSubDepartments.value = [ fs ];
      units.value = [ { text: data.unit.name, value: data.unit.id } ];
      selectedCpv.value = data.cpv as Cpv;
      if (data.manufacturer) {
        manufacturers.value = [ { text: data.manufacturer.name, value: data.manufacturer.id } ];
      }
      if (data.supplier) {
        suppliers.value = [ { text: data.supplier.name, value: data.supplier.id } ];
      }
      if (data.reason) {
        reasons.value = [ { text: data.reason.name, value: data.reason.id } ];
      }
      loaded = true;
    }
  }
  if (loaded) {
    loading.value = false;
    initializing.value = false;
  } else {
    await Router.push({ name: "NotFound" });
  }
});

watch(() => model.value.subDepartmentId, (v) => {
  if (v !== selectedFinanceSubDepartment.value?.subDepartmentId) {
    const sd = financeSubDepartments.value.find(fs => fs.subDepartmentId == v);
    model.value.financeSubDepartmentId = sd?.id;
    selectedFinanceSubDepartment.value = sd;
  }
});
watch(isManufacturerSelected, (newVal) => {
  if (!newVal) {
    model.value.catNum = undefined;
  }
});
watch(isSupplierSelected, (newVal) => {
  if (!newVal) {
    model.value.reasonId = 0;
  }
});

async function getManufacturersAsync() {
  loading.value = true;
  const response = await ManufacturersApi.getList(1, 1000, undefined, "Name", false);
  if (response.success) {
    manufacturers.value = [ getDefaultSelectItem(), ...response.data!.collection.map(d => {
      return { text: d.name, value: d.id } as SelectItem<number>;
    }) ];
  }
  loading.value = false;
}

async function getSuppliersAsync() {
  loading.value = true;
  const response = await SuppliersApi.getList(1, 1000, undefined, "Name", false);
  if (response.success) {
    suppliers.value = [ getDefaultSelectItem(), ...response.data!.collection.map(d => {
      return { text: d.name, value: d.id } as SelectItem<number>;
    }) ];
  }
  loading.value = false;
}

async function getReasonsForSupplierChoiceAsync() {
  loading.value = true;
  const response = await ReasonsForSupplierChoiceApi.getList(1, 1000, undefined, "Name", false);
  if (response.success) {
    reasons.value = response.data!.collection.map(d => {
      return { text: d.name, value: d.id } as SelectItem<number>;
    });
  }
  loading.value = false;
}

async function getUnitsAsync() {
  loading.value = true;
  const response = await UnitsApi.getList(1, 1000, undefined, "Name", false);
  if (response.success) {
    units.value = response.data!.collection.map(d => {
      return { text: d.name, value: d.id } as SelectItem<number>;
    });
  }
  loading.value = false;
}

async function getFinanceSubDepartmentsListAsync() {
  if (!model.value.subDepartmentId) {
    financeSubDepartments.value = [];
    model.value.financeSubDepartmentId = 0;
    return;
  }
  loading.value = true;
  const response = await FinanceSubDepartmentsApi.getListBySubDepartmentId(
      model.value.departmentId!, model.value.subDepartmentId, session.year,
      1, 1000, undefined, undefined, undefined);
  if (response.success) {
    financeSubDepartments.value = response.data!.collection;
  }
  loading.value = false;
}

function getCreateRequest(model: OrderModel) {
  return {
    description: model.description,
    catNum: model.catNum,
    onePrice: model.taxIncluded ? model.onePrice : model.onePrice * 1.2,
    amount: model.amount,
    type: model.type,
    kekv: model.kekv,
    procurementStartDate: model.procurementStartDate ? formatAsDate(model.procurementStartDate) : undefined,
    unitId: model.unitId,
    cpvId: model.cpvId,
    financeSubDepartmentId: model.financeSubDepartmentId,
    manufacturerId: model.manufacturerId > 0 ? model.manufacturerId : undefined,
    supplierId: model.supplierId > 0 ? model.supplierId : undefined,
    reasonId: model.reasonId > 0 ? model.reasonId : undefined,
  } as CreateOrderRequest;
}

function getUpdateRequest(model: OrderModel) {
  const request = getCreateRequest(model) as UpdateOrderRequest;
  request.id = model.id!;
  return request;
}

async function saveAsync() {
  externalErrors.value = {} as Object<string[]>;
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) {
    return;
  }
  loading.value = true;
  const response = await (model.value.id ?? 0 > 0 ? OrderApi.update(getUpdateRequest(model.value)) : OrderApi.create(getCreateRequest(model.value)));
  if (response.success) {
    await Router.push({ name: "FinanceSubDepartmentsOrders",
      params: {
        financeId: selectedFinanceSubDepartment.value!.financeSourceId,
        subDepartmentId: model.value.subDepartmentId,
        type: model.value.type,
      },
    });
    return;
  } else {
    if (response.error?.errors) {
      externalErrors.value = response.error.errors;
    }
    loading.value = false;
  }
}

function getDefaultModel(): OrderModel {
  return {
    id: undefined,
    departmentId: user.departmentId,
    subDepartmentId: user.subDepartmentId,
    financeSubDepartmentId: undefined,
    amount: 0,
    onePrice: 0,
    taxIncluded: true,
    catNum: "",
    cpvId: undefined,
    description: "",
    kekv: "2210",
    manufacturerId: -1,
    procurementStartDate: undefined,
    reasonId: -1,
    supplierId: -1,
    type: 1,
    unitId: undefined,
  };
}

function mapToModel(order: Order): OrderModel {
  return {
    amount: order.amount,
    catNum: order.catNum,
    cpvId: order.cpv.id,
    departmentId: order.department.id,
    description: order.description,
    financeSubDepartmentId: order.financeSubDepartment.id,
    id: order.id,
    kekv: order.kekv ?? "2210",
    manufacturerId: order.manufacturer?.id ?? -1,
    onePrice: order.onePrice,
    taxIncluded: true,
    procurementStartDate: order.procurementStartDate ? new Date(order.procurementStartDate) : undefined,
    reasonId: order.reason?.id ?? -1,
    subDepartmentId: order.subDepartment.id,
    supplierId: order.supplier?.id ?? -1,
    type: order.type,
    unitId: order.unit.id,
    editor: order.editor,
    editedDate: order.editedDate,
  };
}

function getDefaultSelectItem(): SelectItem<number> {
  return { text: t("anyItem"), value: -1 };
}

function getFinanceLabel(fd: FinanceSubDepartment) {
  let amount = 0;
  switch (model.value.type) {
    case OrderType.Equipment:
      amount = fd.leftEquipment;
      break;
    case OrderType.Material:
      amount = fd.leftMaterials;
      break;
    case OrderType.Service:
      amount = fd.leftServices;
      break;
    default:
      throw new Error(`Unknown Order Type: ${ model.value.type }`);
  }
  return `${ fd.financeSourceNumber } - ${ fd.financeSource } (${ t("available") }: ${ currency(amount).format() })`;
}

function onCpvSelected(cpv: Cpv) {
  selectedCpv.value = cpv;
  model.value.cpvId = cpv.id;
  selectCpvDialog.value = false;
}

function onFinanceSubDepartmentSelected(fs: FinanceSubDepartment){
  selectedFinanceSubDepartment.value = fs;
  model.value.financeSubDepartmentId = fs.id;
}

interface OrderModel {
  id: number | undefined;
  description: string;
  catNum: string | undefined;
  onePrice: number;
  taxIncluded: boolean;
  amount: number;
  type: number;
  kekv: string | undefined;
  procurementStartDate: Date | undefined;
  unitId: number | undefined;
  cpvId: number | undefined;
  financeSubDepartmentId: number | undefined;
  subDepartmentId: number | undefined;
  departmentId: number | undefined;
  manufacturerId: number;
  supplierId: number;
  reasonId: number;
  editor?: string;
  editedDate?: string;
}

</script>

<i18n locale="en">
{
  "funding": "Funding",
  "order": "Order",
  "supplier": "Supplier",
  "supplierName": "Supplier Name",
  "reasonForSupplierChoice": "Reason for Supplier Choice",
  "price": "Price",
  "priceOfOneUnit": "Price for one unit",
  "amount": "Amount",
  "units": "Packaging",
  "total": "Total",
  "procurement": "Procurement",
  "createOrder": "New Order",
  "editOrder": "Edit Order",
  "financeSource": "Finance Source",
  "type": "Type",
  "description": "Description",
  "available": "Available",
  "anyItem": "Any",
  "manufacturer": "Manufacturer",
  "selectManufacturerFirst": "Select Manufacturer first",
  "selectSupplierFirst": "Select Supplier first",
  "catNum": "Catalogue #",
  "procurementStartDate": "Procurement Start Date",
  "kekv": "ECCE",
  "kekvFull": "Economic Classification Code of Expenses",
  "selectCpv": "Select CPV",
  "lastEdit": "Edited",
  "taxIncluded": "w Tax"
}
</i18n>

<i18n locale="uk">
{
  "funding": "Фінансування",
  "order": "Замовлення",
  "supplier": "Постачальник",
  "supplierName": "Назва постачальника",
  "reasonForSupplierChoice": "Причина вибору постачальника",
  "amount": "Кількість",
  "price": "Вартість",
  "units": "Пакування",
  "total": "Всього",
  "priceOfOneUnit": "Вартість одиниці",
  "procurement": "Дані закупівлі",
  "createOrder": "Нове замовлення",
  "editOrder": "Редагувати замовлення",
  "financeSource": "Тема",
  "type": "Категорія",
  "description": "Опис",
  "available": "Доступно",
  "anyItem": "Будь-який",
  "manufacturer": "Виробник",
  "selectManufacturerFirst": "Виберіть виробника щоб активувати",
  "selectSupplierFirst": "Виберіть постачальника щоб активувати",
  "catNum": "Номер з каталогу",
  "procurementStartDate": "Дата початку закупівлі",
  "kekv": "КЕКВ",
  "kekvFull": "Код Економічної Класифікації Видатків",
  "selectCpv": "Оберіть CPV",
  "lastEdit": "Редаговано",
  "taxIncluded": "з ПДВ"
}
</i18n>