export function onRequest(context, next) {
    context.locals.apiAccessToken = context.cookies.get('apiAccessToken')?.value;
    return next();
};