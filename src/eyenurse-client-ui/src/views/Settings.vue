<template>
  <div class="max-w-7xl mx-auto divide-y divide-gray-200 lg:col-span-9">
    <div class="pt-6 divide-y divide-gray-200">
      <div class="px-4 sm:px-6">
        <div class="flex items-center">
          <h2 class="text-lg leading-6 font-medium text-gray-900">
            {{ t("clientSettings") }}
          </h2>
          <div v-if="loading" class="ml-2 w-6 items-center justify-center">
            <svg className="animate-spin" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="#14b8a6"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
              </path>
            </svg>
          </div>
        </div>
        <ul role="list" class="mt-2 divide-y divide-gray-200 dark:divide-gray-700">
          <SwitchGroup as="li" class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <SwitchLabel as="p" class="text-sm font-medium text-gray-900 dark:text-gray-100" passive>
                {{ t("darkMode") }}
              </SwitchLabel>
            </div>
            <Switch @update:modelValue="toggleDarkMode" v-model="settings.DarkMode" :class="[
              settings.DarkMode ? 'bg-teal-500' : 'bg-gray-200',
              'ml-4 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-sky-500',
            ]">
              <span aria-hidden="true" :class="[
                settings.DarkMode ? 'translate-x-5' : 'translate-x-0',
                'inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200',
              ]" />
            </Switch>
          </SwitchGroup>
          <SwitchGroup as="li" class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <SwitchLabel as="p" class="text-sm font-medium text-gray-900 dark:text-gray-100" passive>
                {{ t("runWhenStarts") }}
              </SwitchLabel>
            </div>
            <Switch @blur="saveSettings" v-model="settings.RunWhenStarts" :class="[
              settings.RunWhenStarts ? 'bg-teal-500' : 'bg-gray-200',
              'ml-4 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-sky-500',
            ]">
              <span aria-hidden="true" :class="[
                settings.RunWhenStarts ? 'translate-x-5' : 'translate-x-0',
                'inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200',
              ]" />
            </Switch>
          </SwitchGroup>
          <SwitchGroup as="li" class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <SwitchLabel as="p" class="text-sm font-medium text-gray-900 dark:text-gray-100" passive>
                {{ t("resetWhenSessionUnlock") }}
              </SwitchLabel>
            </div>
            <div class="flex items-center">
              <input type="text" v-if="settings.ResetWhenSessionUnlock" @blur="saveSettings"
                v-model="settings.ResetTimeout"
                class="w-28 text-center focus:ring-teal-500 focus:border-teal-500 block px-5 sm:text-sm border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
                placeholder="HH:mm:ss" />
              <Switch @blur="saveSettings" v-model="settings.ResetWhenSessionUnlock" :class="[
                settings.ResetWhenSessionUnlock ? 'bg-teal-500' : 'bg-gray-200',
                'ml-4 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-sky-500',
              ]">
                <span aria-hidden="true" :class="[
                  settings.ResetWhenSessionUnlock ? 'translate-x-5' : 'translate-x-0',
                  'inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200',
                ]" />
              </Switch>
            </div>
 
          </SwitchGroup>
          <li class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <p class="text-sm font-medium text-gray-900 dark:text-gray-100">
                {{ t("restInterval") }}
              </p>
            </div>
            <input type="text" @blur="saveSettings" v-model="settings.RestInterval"
              class="w-28 text-center focus:ring-teal-500 focus:border-teal-500 block px-5 sm:text-sm border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
              placeholder="HH:mm:ss" />
          </li>
          <li class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <p class="text-sm font-medium text-gray-900 dark:text-gray-100">
                {{ t("restDuration") }}
              </p>
            </div>
            <input type="text" @blur="saveSettings" v-model="settings.RestDuration"
              class="w-28 text-center focus:ring-teal-500 focus:border-teal-500 block px-5 sm:text-sm border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
              placeholder="HH:mm:ss" />
          </li>
          <li class="py-4 flex items-center justify-between">
            <div class="flex flex-col">
              <p class="text-sm font-medium text-gray-900 dark:text-gray-100">
                {{ t("pauseHotkey") }}
              </p>
            </div>
            <input type="text" @blur="saveSettings" @keydown="recordHotkey" :value="settings.PauseHotkey"
              class="w-28 text-center focus:ring-teal-500 focus:border-teal-500 block px-5 sm:text-sm border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
              placeholder="Ctrl+Alt+P" />
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { onMounted, reactive, ref } from "vue";
import { Switch, SwitchGroup, SwitchLabel } from "@headlessui/vue";
import type { Setting } from "@/lib/models";
import { useI18n } from "vue-i18n";
import * as client from "../lib/client";
import debounce from "lodash/debounce";
const settings = reactive<Setting>({});
const loading = ref(false);
const { t } = useI18n();

const saveSettings = debounce(async () => {
  loading.value = true;
  console.log("Saving settings...");
  var newSettings = await client.SetSettings(settings);
  Object.assign(settings, newSettings); //更新UI
  loading.value = false;
}, 300);

const recordHotkey = (e: KeyboardEvent) => {
  e.preventDefault();
  const parts = [];
  if (e.ctrlKey) parts.push('Ctrl');
  if (e.shiftKey) parts.push('Shift');
  if (e.altKey) parts.push('Alt');
  if (e.metaKey) parts.push('Win');

  let key = e.key;
  // Handle special cases or formatting if needed
  if (key === ' ') key = 'Space';
  if (key.length === 1) key = key.toUpperCase();
  
  // Ignore modifiers as the main key
  if (!['Control', 'Shift', 'Alt', 'Meta'].includes(e.key)) {
    // If Backspace/Delete, clear
    if (key === 'Backspace' || key === 'Delete') {
       settings.PauseHotkey = '';
       saveSettings();
       return;
    }
    parts.push(key);
  } else {
     // If only modifiers are pressed, we can still show them
     // but usually we want a combination. 
     // Let's allow showing incomplete hotkeys (e.g. "Ctrl+") so user knows it's recording
  }

  // If we have at least one part (even just a modifier), update the display
  // But a valid hotkey usually needs a non-modifier.
  // We'll just display whatever is pressed.
  if (parts.length > 0) {
      settings.PauseHotkey = parts.join('+');
      // Verify if valid hotkey? (e.g. has non-modifier)
      // We can leave validation to backend or just let it be.
      // But we probably shouldn't save "Ctrl" as a hotkey.
      // Let's only save if there is a non-modifier key?
      // No, let's just update model. User blurs to save.
  }
};

const toggleDarkMode = (val: boolean) => {
  if (val) {
    document.documentElement.classList.add('dark');
  } else {
    document.documentElement.classList.remove('dark');
  }
  saveSettings();
};

onMounted(async () => {
  var tmp = await client.getSettings();
  console.log(tmp);
  Object.assign(settings, tmp);
  if (settings.DarkMode) {
      document.documentElement.classList.add('dark');
  } else {
      document.documentElement.classList.remove('dark');
  }
});
</script>
