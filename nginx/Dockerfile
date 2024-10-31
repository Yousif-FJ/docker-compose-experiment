FROM nginx:stable-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY www /data/www

EXPOSE 8198

CMD ["nginx", "-g", "daemon off;"]