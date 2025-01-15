#pragma once

#include "number.h"

typedef struct List {
    const size_t capacity;
    i32 count;
    void *data[];
} List;


List *list_new(size_t capacity);

void list_destroy(List *list);
void list_free_items(List *list);

void list_append(List *list, const void *item);
void *list_pop(List *list);
void list_set(List *list, size_t index, const void *item);
void list_remove(List *list, const void *item);
void list_remove_at(List *list, size_t index);
void list_clear(List *list);
// Returns -1 if no match is found
i32 list_index_of(const List *list, const void *item);
void *list_at(const List *list, size_t index);
bool list_contains(const List *list, const void *item);
bool list_is_full(const List *list);