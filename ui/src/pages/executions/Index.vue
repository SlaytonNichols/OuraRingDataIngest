<template>
  <div class="min-h-screen">
    <main class="flex">
      <div class="px-5 flex-grow mt-10">
        <AppBreadcrumb class="my-4 justify-center" name="Executions" />    
        <markdown-page 
          class="flex-wrap"          
          :frontmatter="frontmatterValue.get()">
          <div         
            v-html="renderedMdText.get()" 
            class="markdown-body pt-4">
          </div>      
        </markdown-page>   
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { auth } from "@/auth"
import { useExecutionsStore } from "@/stores/executions"
import { Executions } from "@/dtos"
import { onMounted, reactive, ref } from "vue"
import marked from "markdown-it"
import mdTable from 'json-to-markdown-table'

type FrontMatter = {
  title: string
  summary?: string
  date?: string
}
const frontmatter = ref<FrontMatter>()
const isAdmin = auth?.value?.roles.indexOf('Admin') >= 0
const store = useExecutionsStore()
const executions = ref<Executions[]>([])
const mdHtml = ref<string>()

const frontmatterValue = reactive({
  // getter
  get() {
    return frontmatter.value
  },
  // setter
  set(newValue: FrontMatter) {    
    frontmatter.value = newValue
  }
})

const renderedMdText = reactive({
  // getter
  get() {
    return mdHtml.value
  },
  // setter
  set(newValue: string) {
    
    mdHtml.value = newValue
  }
})

onMounted(async () => {
  if (isAdmin) {
    executions.value = await store.getExecutions()  
    let columns = ['id', 'startDateTime', 'endDateTime', 'startQueryDateTime', 'endQueryDateTime', 'recordsInserted']
    let mdTableString = mdTable(columns, executions.value)
    var md = new marked()
    var renderedMd = md.render(mdTableString)
    renderedMdText.set(renderedMd)
  }
})
</script>