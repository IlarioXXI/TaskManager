import { TaskItem } from "./taskItem.model";
import { User } from "./user.model";

export interface Team {
    id: number;
    name: string;
    taskItems: TaskItem[];
    taskItemsIds : number[];
    usersIds : string[];
    users : User[];
}
