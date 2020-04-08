--
-- PostgreSQL database dump
--

-- Dumped from database version 10.10
-- Dumped by pg_dump version 11.2

-- Started on 2020-04-08 17:37:20

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA IF NOT EXISTS public;


--
-- TOC entry 2946 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 235 (class 1255 OID 119990)
-- Name: create_participant(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.create_participant() RETURNS TABLE(participant_id uuid)
    LANGUAGE plpgsql
    AS $$
	begin
		create extension if not exists "uuid-ossp";
		select uuid_generate_v4() into participant_id;
		insert into public.participant (participant_id) values (participant_id);
		return next;
	end;
$$;


--
-- TOC entry 207 (class 1255 OID 119827)
-- Name: participant_create_person_team_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.participant_create_person_team_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	declare

		_team_id uuid;

	begin

		create extension if not exists "uuid-ossp";

		select uuid_generate_v4() into _team_id;

		insert into team 

		(

			team_id, 

			leader_id, 

			is_person

		)

		values 

		(

			_team_id,

			new.participant_id,

			true

		);

		return new;

	end;

$$;


--
-- TOC entry 208 (class 1255 OID 119828)
-- Name: participant_delete_person_team_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.participant_delete_person_team_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	begin

		delete 
		from 
			team t 
		where 
			t.leader_id = old.participant_id and
			t.is_person;

		return old;

	end;

$$;


--
-- TOC entry 210 (class 1255 OID 119829)
-- Name: team_add_leader_to_teamlist_if_needed_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.team_add_leader_to_teamlist_if_needed_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	declare _cnt int;

	declare _need_cnt bool;

	begin

		select new.leader_id is not null into _need_cnt;

		if (_need_cnt)

		then

			select 

				count(*) into _cnt

			from

				teamlist tl

			where 

				tl.team_id = new.team_id and
				tl.participant_id = new.leader_id;

		end if;

		if (_need_cnt and _cnt = 0)

		then 

			insert into teamlist 

			(

				team_id, 

				participant_id

			)

			values 

			(

				new.team_id,

				new.leader_id

			);

		end if;

		return new;

	end;

$$;


--
-- TOC entry 211 (class 1255 OID 119830)
-- Name: teamlist_check_delete_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.teamlist_check_delete_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	begin

		if 

		(

			select 

				t.is_person

			from 

				team t

			where

				t.team_id = old.team_id and 

				t.leader_id = old.participant_id

		)

		then 

			return null;

		end if;

		return old;

	end;

$$;


--
-- TOC entry 213 (class 1255 OID 119831)
-- Name: teamlist_check_update_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.teamlist_check_update_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	begin

		if 

		(

			select 

				t.is_person

			from 

				team t

			where

				t.team_id = old.team_id and 

				t.leader_id = old.participant_id

		)

		then 

			return null;

		end if;

		return old;

	end;

$$;


--
-- TOC entry 214 (class 1255 OID 119832)
-- Name: teamlist_clean_team_leader_if_needed_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.teamlist_clean_team_leader_if_needed_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

	begin

		update 

			team 

		set 

			leader_id = null 

		where 

			team_id = old.team_id and 

			leader_id = old.participant_id;

		return old;

	end;

$$;


SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 197 (class 1259 OID 119833)
-- Name: contest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contest (
    contest_id uuid NOT NULL
);


--
-- TOC entry 198 (class 1259 OID 119836)
-- Name: contestant; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contestant (
    contest_id uuid NOT NULL,
    team_id uuid NOT NULL,
    is_virtual boolean NOT NULL
);


--
-- TOC entry 199 (class 1259 OID 119839)
-- Name: participant; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.participant (
    participant_id uuid NOT NULL
);


--
-- TOC entry 200 (class 1259 OID 119842)
-- Name: problem; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.problem (
    problem_id uuid NOT NULL
);


--
-- TOC entry 201 (class 1259 OID 119845)
-- Name: problemset; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.problemset (
    problem_id uuid NOT NULL,
    contest_id uuid NOT NULL
);


--
-- TOC entry 202 (class 1259 OID 119848)
-- Name: submission; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.submission (
    submission_id uuid NOT NULL,
    participant_id uuid NOT NULL,
    team_id uuid,
    problem_id uuid NOT NULL
);


--
-- TOC entry 203 (class 1259 OID 119851)
-- Name: team; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.team (
    team_id uuid NOT NULL,
    leader_id uuid,
    is_person boolean DEFAULT false NOT NULL
);


--
-- TOC entry 204 (class 1259 OID 119855)
-- Name: teamlist; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.teamlist (
    team_id uuid NOT NULL,
    participant_id uuid NOT NULL
);


--
-- TOC entry 205 (class 1259 OID 119858)
-- Name: test; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.test (
    test_id uuid NOT NULL,
    problem_id uuid NOT NULL
);


--
-- TOC entry 206 (class 1259 OID 119861)
-- Name: testlist; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.testlist (
    test_id uuid NOT NULL,
    submission_id uuid NOT NULL
);


--
-- TOC entry 2931 (class 0 OID 119833)
-- Dependencies: 197
-- Data for Name: contest; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2932 (class 0 OID 119836)
-- Dependencies: 198
-- Data for Name: contestant; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2933 (class 0 OID 119839)
-- Dependencies: 199
-- Data for Name: participant; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2934 (class 0 OID 119842)
-- Dependencies: 200
-- Data for Name: problem; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2935 (class 0 OID 119845)
-- Dependencies: 201
-- Data for Name: problemset; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2936 (class 0 OID 119848)
-- Dependencies: 202
-- Data for Name: submission; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2937 (class 0 OID 119851)
-- Dependencies: 203
-- Data for Name: team; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2938 (class 0 OID 119855)
-- Dependencies: 204
-- Data for Name: teamlist; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2939 (class 0 OID 119858)
-- Dependencies: 205
-- Data for Name: test; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2940 (class 0 OID 119861)
-- Dependencies: 206
-- Data for Name: testlist; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 2772 (class 2606 OID 119865)
-- Name: contest contest_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contest
    ADD CONSTRAINT contest_pk PRIMARY KEY (contest_id);


--
-- TOC entry 2774 (class 2606 OID 119867)
-- Name: contestant contestant_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contestant
    ADD CONSTRAINT contestant_pk PRIMARY KEY (contest_id, team_id);


--
-- TOC entry 2776 (class 2606 OID 119869)
-- Name: participant participant_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.participant
    ADD CONSTRAINT participant_pk PRIMARY KEY (participant_id);


--
-- TOC entry 2778 (class 2606 OID 119871)
-- Name: problem problem_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.problem
    ADD CONSTRAINT problem_pk PRIMARY KEY (problem_id);


--
-- TOC entry 2780 (class 2606 OID 119873)
-- Name: submission submission_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.submission
    ADD CONSTRAINT submission_pk PRIMARY KEY (submission_id);


--
-- TOC entry 2782 (class 2606 OID 119875)
-- Name: team team_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.team
    ADD CONSTRAINT team_pk PRIMARY KEY (team_id);


--
-- TOC entry 2784 (class 2606 OID 119877)
-- Name: teamlist teamlist_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.teamlist
    ADD CONSTRAINT teamlist_pk PRIMARY KEY (team_id, participant_id);


--
-- TOC entry 2786 (class 2606 OID 119879)
-- Name: test test_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.test
    ADD CONSTRAINT test_pk PRIMARY KEY (test_id);


--
-- TOC entry 2788 (class 2606 OID 119881)
-- Name: testlist testlist_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.testlist
    ADD CONSTRAINT testlist_pk PRIMARY KEY (test_id, submission_id);


--
-- TOC entry 2804 (class 2620 OID 119882)
-- Name: team add_leader_to_teamlist_if_needed_after_insert; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER add_leader_to_teamlist_if_needed_after_insert AFTER INSERT ON public.team FOR EACH ROW EXECUTE PROCEDURE public.team_add_leader_to_teamlist_if_needed_trigger();


--
-- TOC entry 2805 (class 2620 OID 119883)
-- Name: team add_leader_to_teamlist_if_needed_after_update_leader; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER add_leader_to_teamlist_if_needed_after_update_leader AFTER UPDATE OF leader_id ON public.team FOR EACH ROW EXECUTE PROCEDURE public.team_add_leader_to_teamlist_if_needed_trigger();


--
-- TOC entry 2806 (class 2620 OID 119884)
-- Name: teamlist check_delete; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER check_delete BEFORE DELETE ON public.teamlist FOR EACH ROW EXECUTE PROCEDURE public.teamlist_check_delete_trigger();


--
-- TOC entry 2807 (class 2620 OID 119885)
-- Name: teamlist check_update; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER check_update BEFORE UPDATE ON public.teamlist FOR EACH ROW EXECUTE PROCEDURE public.teamlist_check_update_trigger();


--
-- TOC entry 2808 (class 2620 OID 119886)
-- Name: teamlist clean_team_leader_if_needed_on_delete; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER clean_team_leader_if_needed_on_delete BEFORE DELETE ON public.teamlist FOR EACH ROW EXECUTE PROCEDURE public.teamlist_clean_team_leader_if_needed_trigger();


--
-- TOC entry 2809 (class 2620 OID 119887)
-- Name: teamlist clean_team_leader_if_needed_on_update; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER clean_team_leader_if_needed_on_update BEFORE UPDATE ON public.teamlist FOR EACH ROW EXECUTE PROCEDURE public.teamlist_clean_team_leader_if_needed_trigger();


--
-- TOC entry 2802 (class 2620 OID 119888)
-- Name: participant create_person_team; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER create_person_team AFTER INSERT ON public.participant FOR EACH ROW EXECUTE PROCEDURE public.participant_create_person_team_trigger();


--
-- TOC entry 2803 (class 2620 OID 119889)
-- Name: participant delete_person_team; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER delete_person_team BEFORE DELETE ON public.participant FOR EACH ROW EXECUTE PROCEDURE public.participant_delete_person_team_trigger();


--
-- TOC entry 2789 (class 2606 OID 119890)
-- Name: contestant contestant_fk_contest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contestant
    ADD CONSTRAINT contestant_fk_contest FOREIGN KEY (contest_id) REFERENCES public.contest(contest_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2790 (class 2606 OID 119895)
-- Name: contestant contestant_fk_team; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contestant
    ADD CONSTRAINT contestant_fk_team FOREIGN KEY (team_id) REFERENCES public.team(team_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2791 (class 2606 OID 119900)
-- Name: problemset problemset_fk_contest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.problemset
    ADD CONSTRAINT problemset_fk_contest FOREIGN KEY (contest_id) REFERENCES public.contest(contest_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2792 (class 2606 OID 119905)
-- Name: problemset problemset_fk_problem; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.problemset
    ADD CONSTRAINT problemset_fk_problem FOREIGN KEY (problem_id) REFERENCES public.problem(problem_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2793 (class 2606 OID 119910)
-- Name: submission submission_fk_participant; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.submission
    ADD CONSTRAINT submission_fk_participant FOREIGN KEY (participant_id) REFERENCES public.participant(participant_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2794 (class 2606 OID 119915)
-- Name: submission submission_fk_problem; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.submission
    ADD CONSTRAINT submission_fk_problem FOREIGN KEY (problem_id) REFERENCES public.problem(problem_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2795 (class 2606 OID 119920)
-- Name: submission submission_fk_team; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.submission
    ADD CONSTRAINT submission_fk_team FOREIGN KEY (team_id) REFERENCES public.team(team_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2796 (class 2606 OID 119925)
-- Name: team team_fk_participant; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.team
    ADD CONSTRAINT team_fk_participant FOREIGN KEY (leader_id) REFERENCES public.participant(participant_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 2797 (class 2606 OID 119930)
-- Name: teamlist teamlist_fk_participant; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.teamlist
    ADD CONSTRAINT teamlist_fk_participant FOREIGN KEY (participant_id) REFERENCES public.participant(participant_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2798 (class 2606 OID 119935)
-- Name: teamlist teamlist_fk_team; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.teamlist
    ADD CONSTRAINT teamlist_fk_team FOREIGN KEY (team_id) REFERENCES public.team(team_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2799 (class 2606 OID 119940)
-- Name: test test_fk_problem; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.test
    ADD CONSTRAINT test_fk_problem FOREIGN KEY (problem_id) REFERENCES public.problem(problem_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2800 (class 2606 OID 119945)
-- Name: testlist testlist_fk_submission; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.testlist
    ADD CONSTRAINT testlist_fk_submission FOREIGN KEY (submission_id) REFERENCES public.submission(submission_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2801 (class 2606 OID 119950)
-- Name: testlist testlist_fk_test; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.testlist
    ADD CONSTRAINT testlist_fk_test FOREIGN KEY (test_id) REFERENCES public.test(test_id) ON UPDATE CASCADE ON DELETE CASCADE;


-- Completed on 2020-04-08 17:37:21

--
-- PostgreSQL database dump complete
--

