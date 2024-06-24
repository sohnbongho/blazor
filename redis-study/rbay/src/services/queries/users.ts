import type { CreateUserAttrs } from '$services/types';
import { genId } from '$services/utils';
import { client } from '$services/redis';
import { usersKey, usernamesUniqueKey, userNamesKey } from '$services/keys';
import { attr } from 'svelte/internal';

export const getUserByUsername = async (username: string) => {
    // Use teh username argument to look up the persons Uer Id
    // with the usernames sorted set;
    const decimalId = await client.zScore(userNamesKey(), username);
      
    // make suer we actually got an ID from the lookup
    if(!decimalId){
        throw new Error('User does not exist');
    }

    // Take the id and covert it back to hex
    const id = decimalId.toString(16);

    // User the id to look up the users'hash
    const user = await client.hGetAll(usersKey(id));

    // deserialize and return the hash
    return deserialize(id, user);

};

export const getUserById = async (id: string) => {

    const user = await client.hGetAll(usersKey(id));

    return deserialize(id, user);
};

export const createUser = async (attrs: CreateUserAttrs) => {
    const id = genId();

    // See if the username is already in the set of usernames
    const exists = await client.sIsMember(usernamesUniqueKey(), attrs.username);
    if(exists){
        throw new Error('Username is taken');
    }

    await client.hSet(usersKey(id), serialize(attrs));
    await client.SADD(usernamesUniqueKey(), attrs.username);
    await client.zAdd(userNamesKey(), {
        value: attrs.username, 
        score: parseInt(id, 16)
    });

    return id;
};

const serialize = (user: CreateUserAttrs) => {
    return {
        username: user.username,
        password: user.password

    }
}

const deserialize = (id: string, user: {[key : string] : string}) =>{
    return {
        id: id,
        username: user.username,
        password: user.password,
    }
}