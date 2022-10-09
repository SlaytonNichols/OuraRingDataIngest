<template>
  <div class="min-h-screen">
    <main class="flex">
      <div class="px-5 flex-grow mt-10">
        <AppBreadcrumb class="my-4 justify-center" name="Executions" />    
        {{ JSON.stringify(executions.value) }}
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { auth } from "@/auth"
import { useExecutionsStore } from "@/stores/executions"
import { Executions } from "@/dtos"
import { onMounted, ref } from "vue"

const isAdmin = auth?.value?.roles.indexOf('Admin') >= 0
const store = useExecutionsStore()
const executions = ref<Executions[]>([])

onMounted(async () => {
if (isAdmin) {
  executions.value = await store.getExecutions()  
}
})
</script>