/**
 * select_course_units
 *
 * Overview         : Select units for course
 * Returns          : Units
 * Throws           : Exception when no matching course found
 */
CREATE OR REPLACE FUNCTION coursemanagement.select_course_units(
    IN course_id integer
)
    RETURNS TABLE (
      UnitID integer,
      UnitCode smallint,
      Description VARCHAR
    )
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE
AS $BODY$
BEGIN

SET search_path=coursemanagement ;

IF EXISTS
    (
        SELECT "CourseID" FROM course WHERE "CourseID" = course_id
    )
THEN
    RETURN QUERY
        SELECT u."UnitID", u."UnitCode", u."UnitName" as Description
        FROM
            unit u
        INNER JOIN courseunit cu ON u."UnitID"=cu."UnitID"
        INNER JOIN course c ON c."CourseID"=cu."CourseID"
        WHERE 
            cu."CourseID"=course_id;
ELSE
    RAISE 'CourseID does not exist --> %', course_id USING ERRCODE = 'data_exception';
END IF ;

END
$BODY$;
