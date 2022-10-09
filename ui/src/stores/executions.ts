import { acceptHMRUpdate, defineStore } from "pinia"
import { ResponseStatus } from "@servicestack/client"
import { client } from "@/api"
import { QueryExecutions, Execution } from "@/dtos"

export const useExecutionsStore = defineStore('executions', () => {
    // State
    const executions = ref<Execution[]>([])
    const error = ref<ResponseStatus | null>()

    // Getters
    const allExecutions = computed(() => executions.value)


    // Actions
    const getExecutions = async (path: string) => {
        await refreshExecutions()
        return executions.value
    }
    const refreshExecutions = async (errorStatus?: ResponseStatus) => {
        error.value = errorStatus
        const api = await client.api(new QueryExecutions())
        if (api.succeeded) {
            executions.value = api.response!.results ?? []
        }
    }

    return {
        error,
        allExecutions,
        refreshExecutions,
        getExecutions
    }
})

if (import.meta.hot)
    import.meta.hot.accept(acceptHMRUpdate(useExecutionsStore, import.meta.hot))
