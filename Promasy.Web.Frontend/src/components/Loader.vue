<template>
	<div class="grid">
		<div class="col-12">
			<div class="card">
				<h4>{{ t('loading') }}...</h4>
			</div>
		</div>
	</div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n({ useScope: "local" });
</script>

<i18n locale="en">
{
  "loading": "Loading"
}
</i18n>

<i18n locale="uk">
{
  "loading": "Завантаження"
}
</i18n>