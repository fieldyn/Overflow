import { addToast } from "@heroui/react";

export function errorToast(error: { message: string; status: number }) {
    addToast({
        title: error.status ? `Error ${error.status}` : 'Error!',
        description: error.message,
        color: "danger",
    });
}

export function handleError(error: { message: string; status: number }) {
    if (error.status === 500) throw new Error(error.message);

    errorToast(error);
}
