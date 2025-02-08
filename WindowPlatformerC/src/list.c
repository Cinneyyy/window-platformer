#include "list.h"
#include "stdlib.h"

#pragma region Constr. / Destr.
List *list_new(size_t capacity) {
    List *list = (List*)malloc(sizeof(List) + capacity * sizeof(void*));

    if(!list) {
        return NULL; // TODO: error logging
    }

    *(size_t*)&list->capacity = capacity;
    list->count = 0;

    for(size_t i = 0; i < capacity; i++) {
        list->data[i] = NULL;
    }

    return list;
}

void list_destroy(List *list) {
    free(list);
}

void list_free_all(List *list) {
    for(size_t i = 0; i < list->count; i++) {
        free(list->data[i]);
        list->data[i] = NULL;
    }

    list->count = 0;
}

void list_clear(List *list) {
    for(size_t i = 0; i < list->count; i++) {
        list->data[i] = NULL;
    }

    list->count = 0;
}
#pragma endregion

#pragma region Operations
void list_append(List *list, const void *item) {
    if(list->count >= list->capacity) {
        return; // TODO: error logging
    }

    list->data[list->count] = (void*)item;
    list->count++;
}

void *list_pop(List *list) {
    if(list->count == 0) {
        return NULL; // TODO: error logging
    }

    list->count--;
    void *item = list->data[list->count];
    list->data[list->count] = NULL;
    return item;
}

void list_set(List *list, size_t index, const void *item) {
    if(index >= list->capacity) {
        return; // TODO: error logging
    }

    if(index >= list->count) {
        list->count = index + 1;
    }

    list->data[index] = (void*)item;
}

void list_remove(List *list, const void *item) {
    i32 index = list_index_of(list, item);

    if(index == -1) {
        return; // TODO: error logging
    }

    list_remove_at(list, (size_t)index);
}

void list_remove_at(List *list, size_t index) {
    if(index >= list->count) {
        return; // TODO: error logging
    }

    for(size_t i = index; i < list->count-1; i++) {
        list->data[i] = list->data[i+1];
    }

    list->count--;
    list->data[list->count] = NULL;
}

i32 list_index_of(const List *list, const void *item) {
    for(i32 i = 0; i < list->count; i++) {
        if(list->data[i] == item) {
            return i;
        }
    }

    return -1;
}

void *list_at(const List *list, size_t index) {
    if(index >= list->count) {
        return NULL; // TODO: error logging
    }

    return list->data[index];
}

bool list_contains(const List *list, const void *item) {
    for(size_t i = 0; i < list->count; i++) {
        if(list->data[i] == item) {
            return true;
        }
    }

    return false;
}

bool list_is_full(const List *list) {
    return list->count == list->capacity;
}
#pragma endregion