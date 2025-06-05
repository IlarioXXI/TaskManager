import { Comment } from './comment.model';

export interface TaskItem {
        id: number,
        teamId: number,
        teamName: string,
        statusId: number,
        statusName: string,
        priorityId: number,
        priorityName: string,
        title: string,
        description: string,
        comments: Comment[],
        dueDate: Date,
        appUserId: string,
        color: string,
        dueDateString: string,
}