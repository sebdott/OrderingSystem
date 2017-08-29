FROM mongo:latest
RUN mkdir -p /data/db \
    && chown -R mongodb:mongodb /data/db
VOLUME /data/db
CMD ["mongod", "--dbpath", "/data/db"]