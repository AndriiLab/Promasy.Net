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
import Password from "primevue/password";
import Checkbox from "primevue/checkbox";
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

const app = createApp(App);

app.use(i18n);
app.use(Router);
app.use(Store);

// Prime Vue
app.use(PrimeVue);

app.directive("ripple", Ripple);
app.directive("tooltip", Tooltip);

app.component("InputText", InputText);
app.component("Password", Password);
app.component("Checkbox", Checkbox);
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

app.mount("#app");
