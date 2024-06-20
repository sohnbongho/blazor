import type { CreateItemAttrs } from '$services/types';
import { attr } from 'svelte/internal';
import { create } from 'ts-node';

export const serialize = (attrs: CreateItemAttrs) => {
    return {
        ...attrs,
        createdAt: attrs.createdAt.toMillis(),
        endingAt : attrs.endingAt.toMillis()
    };
};
