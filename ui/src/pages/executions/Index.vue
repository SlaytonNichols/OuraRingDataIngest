<template>
  <div class="min-h-screen">
    <main class="flex">
      <div class="px-5 flex-grow mt-10">
        <AppBreadcrumb class="my-4 justify-center" name="Executions" />    
        {{ executions.value }}
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { auth } from "@/auth"
import { useExeciutionsStore } from "@/stores/executions"
import { Executions } from "@/dtos"
import { ref } from "vue"

const isAdmin = auth?.value?.roles.indexOf('Admin') >= 0
const store = useExeciutionsStore()
const executions = ref<Executions[]>([])
if(isAdmin){
  executions.value = store.getExecutions()
}
</script>