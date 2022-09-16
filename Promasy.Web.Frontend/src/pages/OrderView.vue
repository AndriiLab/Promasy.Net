<template>
  <Loader v-if="loading"></Loader>

  <div v-else class="grid">
    <div class="col-12">
      <div class="card">
        <div class="p-fluid formgrid grid">
          <div class="field col-12">
            <h4>{{ t('funding') }}</h4>
          </div>

          <div class="field col-12">
            <h4>{{ t('order') }}</h4>
          </div>

          <div class="field col-12">
            <h4>{{ t('supplier') }}</h4>
          </div>

          <div class="field col-12">
            <h4>{{ t('amount') }}</h4>
          </div>

          <div class="field col-12">
            <h4>{{ t('procurement') }}</h4>
          </div>
          
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { email, maxLength, numeric, required } from "@/i18n/validators";
import { Order } from "@/services/api/orders";
import { ref, onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import useVuelidate from "@vuelidate/core";
import Loader from "@/components/Loader.vue";

const { t, d } = useI18n({ useScope: "local" });

const loading = ref(false);
const externalErrors = ref({} as Object<string[]>);
const model = ref({} as Order);
const rules = computed(() => {
  return {
    departmentId: { required },
    subDepartmentId: { required },
    financeSubDepartmentId: { required },
    type: { required },
    manufacturerId: { required },
    catNum: { maxLength: maxLength(100) },
  };
});
const v$ = useVuelidate(rules, model, { $lazy: true });

</script>

<i18n locale="en">
{
  "funding": "Funding",
  "order": "Order",
  "supplier": "Supplier",
  "amount": "Amount",
  "procurement": "Procurement"
}
</i18n>

<i18n locale="uk">
{
  "funding": "Фінансування",
  "order": "Замовлення",
  "supplier": "Дані постачальника",
  "amount": "Кількість",
  "procurement": "Дані закупівлі"
}
</i18n>