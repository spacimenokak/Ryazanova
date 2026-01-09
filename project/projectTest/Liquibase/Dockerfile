FROM liquibase/liquibase:latest

COPY change-sets /liquibase/changelog/change-sets
COPY master.xml /liquibase/changelog/

CMD ["--changeLogFile=changelog/master.xml", "--url=jdbc:postgresql://db:5432/test_db", "--username=test_db", "--password=test_db", "update"]
