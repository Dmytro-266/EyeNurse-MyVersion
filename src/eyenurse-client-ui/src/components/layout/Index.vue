<template>
  <div class="flex flex-col bg-gray-50 dark:bg-gray-900 min-h-screen">
    <SliderNav
      :navItems="topNav"
      :bottomItems="bottomNav"
      :open="sidebarOpen"
      @onOpenChanged="onOpenChanged"
    />
    <div class="flex-1">
      <div class="min-h-screen mx-auto flex flex-col">
        <!-- header  -->
        <div
          class="flex px-4 top-0 flex-shrink-0 h-16 bg-white dark:bg-gray-800 shadow-sm ring-1 ring-gray-900 ring-opacity-5 dark:ring-gray-700"
        />
        <main :class="['flex-1', sidebarOpen ? 'md:pl-64' : '']">
          <slot></slot>
        </main>
        <Footer :class="['flex-1', sidebarOpen ? 'md:pl-64' : '']" />
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { CogIcon } from "@heroicons/vue/outline";
import type { NavItemModel } from "./models/NavItemModel";
import SliderNav from "./components/SliderNav.vue";
import Footer from "./components/Footer.vue";

const sidebarOpen = ref<boolean>(false);

const onOpenChanged = (e: boolean) => {
  localStorage.setItem("sidebarOpen", JSON.stringify(e));
  sidebarOpen.value = e;
};

onMounted(() => {
  setTimeout(() => {
    sidebarOpen.value = true;
  }, 100);
});

const topNav = ref<NavItemModel[]>([]);
const bottomNav = ref<NavItemModel[]>([]);
</script>
