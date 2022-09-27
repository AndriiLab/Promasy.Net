import "primevue/resources/themes/lara-light-blue/theme.css";
import "primevue/resources/primevue.min.css";
import "primeflex/primeflex.css";
import "primeicons/primeicons.css";
import "./assets/styles/layout.scss";
import "./assets/flags/flags.css";

import { createApp } from "vue";
import App from "./App.vue";
import { i18n } from "./i18n";
import { Store } from "./store";
import { Router } from "./router";
import PrimeVue from "primevue/config";
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import Password from "primevue/password";
import Checkbox from "primevue/checkbox";
import RadioButton from "primevue/radiobutton";
import Button from "primevue/button";
import Badge from "primevue/badge";
import Ripple from "primevue/ripple";
import Textarea from "primevue/textarea";
import Dropdown from "primevue/dropdown";
import Message from "primevue/message";
import OverlayPanel from "primevue/overlaypanel";
import Tooltip from "primevue/tooltip";
import Tag from "primevue/tag";
import DataTable from "primevue/datatable";
import TreeTable from "primevue/treetable";
import Column from "primevue/column";
import Toast from "primevue/toast";
import Toolbar from "primevue/toolbar";
import Menu from "primevue/menu";
import Dialog from "primevue/dialog";
import Chip from "primevue/chip";
import Calendar from "primevue/calendar";
import ToastService from "primevue/toastservice";
import BadgeDirective from 'primevue/badgedirective';
import { vue3Debounce } from "vue-debounce";

const app = createApp(App);

app.use(i18n);
app.use(Router);
app.use(Store);

// Prime Vue
app.use(PrimeVue);
app.use(ToastService);

app.directive("ripple", Ripple);
app.directive("tooltip", Tooltip);
app.directive("debounce", vue3Debounce({ lock: true }));
app.directive("badge", BadgeDirective);

app.component("InputText", InputText);
app.component("InputNumber", InputNumber);
app.component("Password", Password);
app.component("Checkbox", Checkbox);
app.component("RadioButton", RadioButton);
app.component("Button", Button);
app.component("Badge", Badge);
app.component("Textarea", Textarea);
app.component("Dropdown", Dropdown);
app.component("Message", Message);
app.component("OverlayPanel", OverlayPanel);
app.component("Tag", Tag);
app.component("DataTable", DataTable);
app.component("TreeTable", TreeTable);
app.component("Column", Column);
app.component("Toast", Toast);
app.component("Toolbar", Toolbar);
app.component("Menu", Menu);
app.component("Dialog", Dialog);
app.component("Chip", Chip);
app.component("Calendar", Calendar);

app.mount("#app");
